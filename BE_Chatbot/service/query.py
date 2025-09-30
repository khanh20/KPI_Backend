from dotenv import load_dotenv
import requests

API_URL = "http://localhost:5006/api/User"  # backend user API
API_URL_KPI = "http://localhost:5118/api" # backend Kpi API


# Hàm helper
from flask import request, jsonify

# --- Hàm helper ---
def call_api(endpoint: str, method="GET", token=None, params=None, data=None, base="user"):
    # Lấy token từ bên fe gửi sang
    header = request.headers.get("Authorization")
    if not header or not header.startswith("Bearer "):
        return {"error": 401, "message": "No token provided"}
    token = header.split(" ")[1]  # Lấy token 
    headers = {"Authorization": f"Bearer {token}"}

    if base == "user":
        url = f"{API_URL}{endpoint}"
    elif base == "kpi":
        url = f"{API_URL_KPI}{endpoint}"
    else:
        raise ValueError("base must be 'user' or 'kpi'")

    # # Debug
    # print("=====================")
    # print("METHOD:", method)
    # print("URL:", url)
    # print("HEADERS:", headers)
    # print("PARAMS:", params)
    # print("DATA:", data)
    # print("=====================")

    resp = requests.request(method, url, headers=headers, params=params, json=data)
    if resp.status_code == 200:
        return resp.json()
    else:
        return {"error": resp.status_code, "message": resp.text}


# --- Auth (User, Role) ---

def get_all_users(token=None):
    """Lấy danh sách tất cả nhân sự, giảng viên"""
    return call_api("", method="GET", token=token)

def get_user_by_id(user_id: int, token=None):
    """Lấy thông tin chi tiết 1 nhân sự theo ID"""
    return call_api(f"/{user_id}", method="GET", token=token)

def search_users(keyword: str, token=None):
    """Tìm kiếm nhân sự theo username, email, firstname hoặc lastname"""
    return call_api("", method="GET", token=token, params={"keyword": keyword})

def get_users_by_role(user_id: int, token=None):
    """
    Lấy danh sách role của một nhân sự theo user_id.
    """
    return call_api(f"/user-roles/{user_id}", method="GET", token=token)

# --- KPI (KPI, Violation,Assignment,Approval) ---
# --- KPI Templates ---
def get_kpi_templates(token=None):
    return call_api("/Kpi/templates", "GET", token=token, base="kpi")

def get_kpi_template_by_id(template_id: int, token=None):
    return call_api(f"/Kpi/templates/{template_id}", "GET", token=token, base="kpi")

def get_template_items(template_id: int, token=None):
    return call_api(f"/Kpi/templates/{template_id}/items", "GET", token=token, base="kpi")

def export_template(template_id: int, token=None):
    return call_api(f"/Kpi/export-template/{template_id}", "GET", token=token, base="kpi")

# --- KPI Items ---
def get_kpi_items(token=None):
    return call_api("/Kpi/items", "GET", token=token, base="kpi")

def get_kpi_item_by_id(item_id: int, token=None):
    return call_api(f"/Kpi/items/{item_id}", "GET", token=token, base="kpi")

def get_kpi_items_by_creator(token=None):
    return call_api("/Kpi/items/creator", "GET", token=token, base="kpi")
