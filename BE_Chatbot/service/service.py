import os
from google import genai
from google.genai import types
from .guide import GUIDE_CONTENT, GUIDE_DETAILS, FAQ  
from .query import get_all_users,get_user_by_id,get_users_by_role,search_users, get_kpi_item_by_id,get_kpi_items,get_kpi_items_by_creator,get_kpi_template_by_id,get_kpi_templates,get_template_items,export_template
client = genai.Client(api_key=os.getenv("GEMINI_API_KEY"))

# Định nghĩa schema cho các function
tools = [
    types.Tool(
        function_declarations=[
            types.FunctionDeclaration(
                name="get_user_count",
                description="Đếm tổng số nhân sự, giảng viên trong trường"
            ),
            types.FunctionDeclaration(
                name="get_all_users",
                description="Lấy danh sách tất cả nhân sự, giảng viên"
            ),
            types.FunctionDeclaration(
                name="get_user_by_id",
                description="Lấy thông tin chi tiết nhân sự, giảng viên theo ID",
                parameters={
                    "type": "object",
                    "properties": {"user_id": {"type": "integer"}},
                    "required": ["user_id"]
                }
            ),
            types.FunctionDeclaration(
                name="search_users",
                description="Tìm kiếm nhân sự, giảng viên theo tên hoặc email",
                parameters={
                    "type": "object",
                    "properties": {"keyword": {"type": "string"}},
                    "required": ["keyword"]
                }
            ),
            types.FunctionDeclaration(
                name="get_users_by_role",
                description="Lấy danh sách vai trò của nhân sự theo user_id",
                parameters={
                    "type": "object",
                    "properties": {
                        "user_id": {"type": "integer"}
                    },
                    "required": ["user_id"]
                }
            ),
            types.FunctionDeclaration(
                name="get_kpi_templates",
                description="Lấy tất cả template có trong hệ thống",   
            ),
            types.FunctionDeclaration(
                name="get_kpi_template_by_id",
                description="Lấy thông tin chi tiết KPI template theo ID",
                parameters={
                    "type": "object",
                    "properties": {"template_id": {"type": "integer"}},
                    "required": ["template_id"]
                }
            ),
            types.FunctionDeclaration(
                name="get_kpi_items",
                description="Lấy danh sách tất cả KPI items"
            ),
            types.FunctionDeclaration(
                name="get_kpi_item_by_id",
                description="Lấy thông tin chi tiết KPI item theo ID",
                parameters={
                    "type": "object",
                    "properties": {"item_id": {"type": "integer"}},
                    "required": ["item_id"]
                }
            )
        ]
    )
]

# Map tên hàm Gemini → API thực tế
FUNCTION_MAP = {
    # User
    "get_all_users": get_all_users,
    "get_user_by_id": get_user_by_id,
    "search_users": search_users,
    "get_users_by_role": get_users_by_role,
    "get_user_count": lambda token=None: {"count": len(get_all_users(token=token) or [])},

    # KPI
    "get_kpi_templates": get_kpi_templates,
    "get_kpi_template_by_id": get_kpi_template_by_id,
    "get_template_items": get_template_items,
    "get_kpi_items": get_kpi_items,
    "get_kpi_item_by_id": get_kpi_item_by_id,
    "get_kpi_items_by_creator": get_kpi_items_by_creator,
    "export_template": export_template,
}

# Hàm format form trả lời của gemini
def format_result(result):
    if isinstance(result, list) and not result:
        return "Không có dữ liệu."
    if isinstance(result, list) and isinstance(result[0], dict):
        return "\n".join(
            ["- " + "; ".join([f"{k}: {v}" for k, v in item.items()]) for item in result]
        )
    if isinstance(result, list):
        return "\n".join(f"- {v}" for v in result)
    if isinstance(result, dict):
        return "\n".join(f"- {k}: {v}" for k, v in result.items())
    return f"- {str(result)}"

MAX_HISTORY = 5
conversation = []
def ask_gemini(user_question: str, token: str = None):
    # Lưu câu hỏi mới vào lịch sử
    conversation.append({"role": "user", "content": user_question})

    # Lấy 5 lượt hội thoại gần nhất (user + assistant)
    short_history = conversation[-MAX_HISTORY*2:]

    # Chuẩn bị nội dung gửi lên Gemini
    contents = [
        f"Bạn là trợ lý hỗ trợ cho web KPI.\n\n"
        f"Đây là tài liệu hướng dẫn chi tiết:\n"
        f"{GUIDE_CONTENT}\n{GUIDE_DETAILS}\n{FAQ}\n\n"
        f"Nhiệm vụ:\n"
        f"- Nếu câu hỏi liên quan tới dữ liệu thì dùng tool.\n"
        f"- Nếu câu hỏi liên quan đến cách sử dụng app (các bước đăng nhập, tạo KPI, phê duyệt, giao KPI, ...) "
        f"thì trả lời trực tiếp dựa vào GUIDE_CONTENT & GUIDE_DETAILS & FAQ.\n"
    ]

    # Thêm lịch sử vào contents
    for msg in short_history:
        role = "Người dùng" if msg["role"] == "user" else "Trợ lý"
        contents.append(f"{role}: {msg['content']}")

    # Gọi Gemini với lịch sử
    response = client.models.generate_content(
        model="gemini-2.5-flash",
        contents="\n".join(contents),
        config=types.GenerateContentConfig(
            temperature=0,
            tools=tools
        )
    )

    # Kiểm tra function_call
    function_call = getattr(response.candidates[0].content.parts[0], "function_call", None)

    if function_call:
        fn_name = function_call.name
        args = function_call.args or {}
        fn = FUNCTION_MAP.get(fn_name)
        if not fn:
            return f"Không tìm thấy function {fn_name}"

        # Gọi hàm với token
        result = fn(**args, token=token) if args else fn(token=token)
        formatted_result = format_result(result)

        # Gọi lại Gemini để trả lời dựa vào dữ liệu
        follow_up = client.models.generate_content(
            model="gemini-2.5-flash",
            contents=f"""
                Người dùng hỏi: {user_question}
                Dữ liệu kết quả:
                {formatted_result}
                Hãy trả lời câu hỏi dựa trên dữ liệu trên, giữ nguyên định dạng dữ liệu khi cần.
                """,
            config=types.GenerateContentConfig(temperature=0)
        )
        answer = getattr(follow_up, "text", formatted_result)
        conversation.append({"role": "assistant", "content": answer})
        return answer

    # Nếu không gọi function, lấy text trả lời từ Gemini
    answer = getattr(response, "text", "Không có phản hồi từ Gemini")
    conversation.append({"role": "assistant", "content": answer})
    return answer
