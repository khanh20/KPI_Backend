from flask import Flask, request, jsonify
from pydantic import BaseModel
from google import genai
from google.genai import types
import pyodbc
from flask_cors import CORS

app = Flask(__name__)

CORS(app, resources={r"/*": {"origins": "*"}}) 

GEMINI_API_KEY = "AIzaSyCfd0bYsf-dTjTlPG6pS8DXKqgdCSb_ofE"
client = genai.Client(api_key=GEMINI_API_KEY)

# --- Kết nối SQL Server ---
DB_CONN_STR = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=THANHMOI\\SQLEXPRESS;"   # server name thật
    "DATABASE=KPICoreDb;"            # DB chứa VIEW UserKpiSummary
    "Trusted_Connection=yes;"
    "Encrypt=yes;"
    "TrustServerCertificate=yes;"
)

class ChatRequest(BaseModel):
    message: str

# --- Lấy data từ view vi phạm ---
def fetch_violation_data():
    with pyodbc.connect(DB_CONN_STR) as conn:
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM UserUnitViolation")
        columns = [col[0] for col in cursor.description]
        rows = cursor.fetchall()
    return [dict(zip(columns, row)) for row in rows]

# --- Lấy data từ view ---
def fetch_view_data():
    # Sử dụng 'with' để đảm bảo kết nối được đóng tự động
    with pyodbc.connect(DB_CONN_STR) as conn: 
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM UserKpiSummary")
        columns = [col[0] for col in cursor.description]
        rows = cursor.fetchall()
    result = []
    for row in rows:
        result.append(dict(zip(columns, row))) 
    return result

# --- Build context từ view ---
def build_context(view_data,violation_data):
    context = "DỮ LIỆU KPI\n"
    for row in view_data:
        context += (
            f"Người dùng: {row['Họ tên']}, "
            f"Đơn vị: {row['Tên đơn vị']}, "
            f"Tên chức vụ: {row['Tên chức vụ']},"
            f"Trưởng đơn vị: {row['Trưởng đơn vị']}, "
            f"KPI: {row['KpiName']}, "
            f"Mục tiêu: {row['Mục tiêu']}, "
            f"Kết quả thực tế: {row['Kết quả thực tế']}, "
            f"Điểm KPI: {row['Điểm KPI']}, "
            f"Trạng thái: {row['Trạng thái KPI']}, "
            f"Điểm KPI loại Chức năng: {row['Điểm KPI Chức năng']}, "
            f"Điểm KPI loại Mục tiêu: {row['Điểm KPI Mục tiêu']}, "
            f"Điểm KPI loại Kỷ luật: {row['Điểm KPI kỉ luật']}, "
            f"Tổng điểm KPI cá nhân: {row['Tổng điểm KPI cá nhân']}\n"
        )

    context += "\n=== DỮ LIỆU VI PHẠM ===\n"
    for row in violation_data:
        context += (
            f"Người dùng: {row['Họ tên']}, "
            f"Đơn vị: {row['Tên đơn vị']}, "
            f"Vi phạm: {row['Tên Vi phạm']}, "
            f"Điểm trừ: {row['Điểm trừ']}, "
            f"Số lần vi phạm: {row['SoLanViPham']}, "
            f"Điểm vi phạm phạm tổng: {row['Điểm trừ final']}\n"
        )

    return context


# --- Endpoint Chat ---
@app.route("/chat", methods=["POST"])
def chat():
    try:
        data = request.get_json()
        if not data or "message" not in data:
            return jsonify({"error": "Missing 'message' in request body"}), 400
        message = data["message"]

        # Lấy dữ liệu từ DB
        view_data = fetch_view_data()
        violation_data = fetch_violation_data()
    except Exception as e:
        return jsonify({"error": f"DB error: {e}"}), 500

    context = build_context(view_data, violation_data)

    prompt = (
        f"Bạn là trợ lý trả lời câu hỏi KPI. "
        f"Hãy trả lời gọn gàng, súc tích, không tiết lộ ID. Trình bày thông minh biết xuống dòng và ngắt dòng hợp lý. "
        f"Dữ liệu KPI \n{context}\n"
        f"Câu hỏi: {message}"
    )

    try:
        response = client.models.generate_content(
            model="gemini-2.5-flash",
            contents=prompt,
            config=types.GenerateContentConfig(
                thinking_config=types.ThinkingConfig(thinking_budget=5)
            ),
        )
        return jsonify({"answer": response.text})
    except Exception as e:
        return jsonify({"error": f"Gemini API error: {e}"}), 500

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)