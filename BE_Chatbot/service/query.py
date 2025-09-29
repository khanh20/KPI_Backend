from sqlalchemy import create_engine, text
import os
from dotenv import load_dotenv

# Load biến môi trường từ .env
load_dotenv()

# Kết nối SQL Server
DB_URL_KPI = os.getenv("DB_URL_KPI")
DB_URL_AUTH = os.getenv("DB_URL_AUTH")

engine_kpi = create_engine(DB_URL_KPI)
engine_auth = create_engine(DB_URL_AUTH)


def get_user_count():
    """Đếm tổng số nhân sự"""
    with engine_auth.connect() as conn:
        result = conn.execute(text("SELECT COUNT(*) AS total FROM [User]"))
        row = result.mappings().first()
        return {"total": row["total"]}


def get_all_users():
    """Lấy danh sách tất cả nhân sự"""
    with engine_auth.connect() as conn:
        result = conn.execute(text("""
            SELECT Id, UserName, Email, RoleId
            FROM [User]
        """))
        return [dict(row) for row in result.mappings().all()]


def get_user_by_id(user_id: int):
    """Lấy thông tin chi tiết 1 nhân sự theo ID"""
    with engine_auth.connect() as conn:
        result = conn.execute(text("""
            SELECT *
            FROM [User]
            WHERE Id = :user_id
        """), {"user_id": user_id})
        return result.mappings().first()


def search_users(keyword: str):
    """Tìm kiếm nhân sự theo username, email, firstname hoặc lastname"""
    with engine_auth.connect() as conn:
        result = conn.execute(text("""
            SELECT Id, UserName, Email, RoleId, FirstName, LastName
            FROM [User]
            WHERE UserName LIKE :kw
               OR Email LIKE :kw
               OR FirstName LIKE :kw
               OR LastName LIKE :kw
        """), {"kw": f"%{keyword}%"})
        return [dict(row) for row in result.mappings().all()]


def get_users_by_role(role_name: str):
    """Lọc danh sách nhân sự theo tên vai trò"""
    with engine_auth.connect() as conn:
        result = conn.execute(text("""
            SELECT 
                u.Id,
                u.FirstName + ' ' + u.LastName AS FullName,
                r.Name 
            FROM [User] u
            INNER JOIN [Roles] r ON u.RoleId = r.Id
            WHERE r.Name = :role_name
        """), {"role_name": role_name})
        
        return [dict(row) for row in result.mappings().all()]

