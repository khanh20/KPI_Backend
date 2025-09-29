import os
from flask import Flask, request, jsonify
from flask_cors import CORS
from google import genai
from google.genai import types

app = Flask(__name__)
CORS(app)

# Khởi tạo Gemini client
client = genai.Client(api_key=os.getenv("GEMINI_API_KEY"))

@app.route("/chat", methods=["POST"])
def chat():
    try:
        data = request.get_json()
        prompt = data.get("prompt", "")

        if not prompt:
            return jsonify({"error": "Prompt is required"}), 400

        # Gọi Gemini API
        response = client.models.generate_content(
            model="gemini-2.5-flash",
            contents=prompt,
            config=types.GenerateContentConfig(
                thinking_config=types.ThinkingConfig(thinking_budget=0)  
            ),
        )

        # Lấy text trả về
        text = getattr(response, "text", "Không có phản hồi từ Gemini")
        return jsonify({"text": text})

    except Exception as e:
        print("Gemini error:", e)
        return jsonify({"error": "AI request failed"}), 500

if __name__ == "__main__":
    app.run(port=3000, debug=True)
