# guide.py

GUIDE_CONTENT = """
# HƯỚNG DẪN SỬ DỤNG Web QUẢN LÝ KPI Trường Đại học Xây dựng Hà nội

## 1. Đăng nhập
- Mở ứng dụng.
- Nhập Email và Mật khẩu đã được cấp.
- Nhấn Đăng nhập để vào hệ thống.
- Nếu quên mật khẩu → nhấn Quên mật khẩu để đặt lại.
"""

# Chi tiết theo từng menu
GUIDE_DETAILS = {
    "Trang chủ": """
- Đây là màn hình tổng quan của hệ thống.
- Hiển thị các thông tin chung, thông báo và tình hình KPI hiện tại.
- Người dùng có thể nhanh chóng điều hướng đến các chức năng khác.
""",

    "Tạo KPI": """
- Dùng để tạo mới các chỉ số KPI cho nhân sự hoặc phòng ban.
- Các bước:
  1. Chọn **Tạo KPI**.
  2. Nhập thông tin KPI: tên, mô tả, trọng số, thời gian áp dụng.
  3. Nhấn **Lưu** để hoàn tất.
""",

    "Phê duyệt KPI": """
- Quản lý hoặc cấp trên sẽ xem xét và phê duyệt KPI mà nhân sự đã tạo/gửi lên.
- Các bước:
  1. Vào mục **Phê duyệt KPI**.
  2. Chọn KPI đang chờ duyệt.
  3. Nhấn **Phê duyệt** hoặc **Từ chối**, kèm lý do nếu cần.
""",

    "Template KPI": """
- Dùng để quản lý các mẫu KPI chuẩn có sẵn.
- Tiện cho việc tái sử dụng, tránh nhập lại từ đầu.
- Người dùng có thể:
  - Xem danh sách template.
  - Tạo template mới.
  - Áp dụng template vào nhân sự/phòng ban.
""",

    "Giao KPI": """
- Chức năng phân bổ KPI từ quản lý đến từng nhân sự.
- Các bước:
  1. Chọn **Giao KPI**.
  2. Chọn nhân sự/phòng ban nhận KPI.
  3. Chọn KPI cần giao.
  4. Nhấn **Xác nhận**.
""",

    "Đánh giá cá nhân": """
- Nhân sự tự đánh giá tiến độ hoặc kết quả KPI được giao.
- Các bước:
  1. Vào mục **Đánh giá cá nhân**.
  2. Chọn KPI cần đánh giá.
  3. Nhập điểm số hoặc kết quả thực hiện.
  4. Gửi để quản lý xem xét.
""",

    "Vi phạm": """
- Ghi nhận các trường hợp vi phạm KPI hoặc quy định liên quan.
- Quản lý có thể nhập thông tin vi phạm để lưu lại trong hệ thống.
""",

    "Điểm KPI": """
- Hiển thị tổng điểm KPI của từng nhân sự hoặc phòng ban.
- Người dùng có thể xem:
  - Điểm chi tiết theo từng KPI.
  - Điểm tổng kết theo giai đoạn/tháng/quý.
"""
}

# FAQ – Câu hỏi thường gặp
FAQ = {
    "Làm sao để tạo KPI?": "Bạn vào mục *Tạo KPI*, nhập thông tin (tên, mô tả, trọng số, thời gian áp dụng) rồi nhấn Lưu.",
    "Khác biệt giữa Tạo KPI và Giao KPI là gì?": "- *Tạo KPI*: chỉ tạo mới KPI trong hệ thống. \n- *Giao KPI*: phân bổ KPI đã tạo cho nhân sự hoặc phòng ban cụ thể.",
    "Điểm KPI được tính như thế nào?": "Điểm KPI dựa trên kết quả thực hiện của từng KPI, có thể kèm trọng số cá nhân. Hệ thống sẽ tính theo công thức chung *KPIs = KPI Chức năng + KPI Mục tiêu - KPI Tuân thủ*."
}
