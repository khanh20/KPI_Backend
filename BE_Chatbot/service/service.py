import os
from google import genai
from google.genai import types
from .query import (
    get_user_count,
    get_all_users,
    get_user_by_id,
    search_users,
    get_users_by_role
)
from .guide import GUIDE_CONTENT,GUIDE_DETAILS, FAQ   # 👈 thêm import

client = genai.Client(api_key=os.getenv("GEMINI_API_KEY"))

# Định nghĩa schema cho các function
tools = [
    types.Tool(
        function_declarations=[
            types.FunctionDeclaration(
                name="get_user_count",
                description="Đếm tổng số nhân sự trong công ty"
            ),
            types.FunctionDeclaration(
                name="get_all_users",
                description="Lấy danh sách tất cả nhân sự"
            ),
            types.FunctionDeclaration(
                name="get_user_by_id",
                description="Lấy thông tin chi tiết nhân sự theo ID",
                parameters={
                    "type": "object",
                    "properties": {"user_id": {"type": "integer"}},
                    "required": ["user_id"]
                }
            ),
            types.FunctionDeclaration(
                name="search_users",
                description="Tìm kiếm nhân sự theo tên hoặc email",
                parameters={
                    "type": "object",
                    "properties": {"keyword": {"type": "string"}},
                    "required": ["keyword"]
                }
            ),
            types.FunctionDeclaration(
                name="get_users_by_role",
                description="Lấy danh sách nhân sự theo vai trò",
                parameters={
                    "type": "object",
                    "properties": {"role_name": {"type": "string"}},
                    "required": ["role_name"]
                }
            )
        ]
    )
]

# Map tên hàm Gemini trả về → Python function thực tế
FUNCTION_MAP = {
    "get_user_count": get_user_count,
    "get_all_users": get_all_users,
    "get_user_by_id": get_user_by_id,
    "search_users": search_users,
    "get_users_by_role": get_users_by_role
}

def format_result(result):
    """
    Định dạng kết quả trả về:
    - Mỗi item bắt đầu bằng '- '
    - Mỗi key: value trên một dòng
    - Sau mỗi item, xuống dòng
    """

    # Nếu là list rỗng
    if isinstance(result, list) and not result:
        return "Không có dữ liệu."

    # Nếu là list object (list các dict)
    if isinstance(result, list) and isinstance(result[0], dict):
        return "\n".join(
            ["- " + ", ".join([f"{k}: {v}" for k, v in item.items()]) for item in result]
        )

    # Nếu là list giá trị thường
    if isinstance(result, list):
        return "\n".join(f"- {v}" for v in result)

    # Nếu là dict
    if isinstance(result, dict):
        return "\n".join(f"- {k}: {v}" for k, v in result.items())

    # ép sang string
    return f"- {str(result)}"


def ask_gemini(user_question: str):
    # Bước 1: Gửi câu hỏi user cho Gemini
    response = client.models.generate_content(
        model="gemini-2.5-flash",
        contents=f"""
        Bạn là trợ lý hỗ trợ cho web KPI.      
        Đây là tài liệu hướng dẫn chi tiết:
        {GUIDE_CONTENT} , {GUIDE_DETAILS}
        Nhiệm vụ:
        - Nếu câu hỏi liên quan tới dữ liệu (danh sách user, tìm kiếm, role, ...) thì dùng tool.
        - Nếu câu hỏi liên quan đến cách sử dụng app (các bước đăng nhập, tạo KPI, phê duyệt, giao KPI, ...) thì trả lời trực tiếp dựa vào GUIDE_CONTENT & GUIDE_DETAILS & FAQ.
        - Luôn trả lời bằng tiếng Việt, dễ hiểu.

        Câu hỏi: {user_question}
        """,
        config=types.GenerateContentConfig(
            temperature=0,
            tools=tools
        )
    )

    # Bước 2: Kiểm tra Gemini có yêu cầu gọi hàm không
    function_call = getattr(
        response.candidates[0].content.parts[0], "function_call", None
    )

    if function_call:
        fn_name = function_call.name
        args = function_call.args or {}

        fn = FUNCTION_MAP.get(fn_name)
        if not fn:
            return f"Không tìm thấy function {fn_name}"

        # Bước 3: Thực thi hàm thật
        result = fn(**args) if args else fn()
        formatted_result = format_result(result)  # format answers

        # Bước 4: Gửi lại kết quả cho Gemini để soạn câu trả lời
        follow_up = client.models.generate_content(
            model="gemini-2.5-flash",
            contents=f"""
                Người dùng hỏi: {user_question}
                Dữ liệu kết quả (đã format):
                {formatted_result}
                Hãy trả lời câu hỏi dựa trên dữ liệu trên, giữ nguyên định dạng dữ liệu khi cần.
                """,
        config=types.GenerateContentConfig(temperature=0)
            )
        return getattr(follow_up, "text", formatted_result)

    return getattr(response, "text", "Không có phản hồi từ Gemini")
