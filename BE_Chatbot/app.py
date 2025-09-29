from flask import Flask, request, jsonify
from flask_cors import CORS
from service.service import ask_gemini

app = Flask(__name__)
CORS(app)

@app.route("/chat", methods=["POST"])
def chat():
    try:
        data = request.get_json()
        prompt = data.get("prompt", "")

        if not prompt:
            return jsonify({"error": "Prompt is required"}), 400

        text = ask_gemini(prompt)
        return jsonify({"text": text})

    except Exception as e:
        print("Gemini error:", e)
        return jsonify({"error": "AI request failed"}), 500

if __name__ == "__main__":
    app.run(port=3000, debug=True)
