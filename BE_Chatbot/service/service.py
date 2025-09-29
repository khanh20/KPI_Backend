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
                    "properties": {"role": {"type": "string"}},
                    "required": ["role"]
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
    # Nếu là list object (danh sách user, role, ...)
    if isinstance(result, list) and result:
        if isinstance(result[0], dict):
            # Nếu có field UserName thì coi như list user
            if "UserName" in result[0]:
                return format_users_table(result)

            # Còn lại -> in ra từng dict
            return "\n\n".join(
                [", ".join([f"{k}: {v}" for k, v in item.items()]) for item in result]
            )

    # Nếu chỉ 1 dict
    if isinstance(result, dict):
        return "\n".join([f"{k}: {v}" for k, v in result.items()])

    # Nếu chỉ 1 giá trị đơn giản
    return str(result)


def ask_gemini(user_question: str):
    # Bước 1: Gửi câu hỏi user cho Gemini
    response = client.models.generate_content(
        model="gemini-2.5-flash",
        contents=user_question,
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

        # Bước 4: Gửi lại kết quả cho Gemini để soạn câu trả lời
        follow_up = client.models.generate_content(
            model="gemini-2.5-flash",
            contents=[
                user_question,
                types.Part.from_function_response(
                    name=fn_name,
                    response={"result": result}
                )
            ]
        )
        return getattr(follow_up, "text", "Không có phản hồi từ Gemini")

    # Nếu không có function_call → trả lời trực tiếp
    return getattr(response, "text", "Không có phản hồi từ Gemini")
