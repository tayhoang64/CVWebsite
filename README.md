
<h1 align="center">Đồ án cuối kì lập trình web</h1>

## 1. Hướng dẫn chạy project lần đầu
Clone project về:

    git clone https://github.com/tayhoang64/CVWebsite.git

Sau khi clone về 
1. Tìm file `appsettings.json.example`, sau đó copy 1 bản
2. Sửa tên file đã copy thành `appsettings.json`
3. Mở file `appsettings.json` ra sẽ thấy nội dung như sau

```c#
{
  "ConnectionStrings": {
    "CVConnection": "connection_string"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
4. Thay thế `connection_string` thành chuỗi connection string tương ứng.
5. Chuỗi connection string có dạng:
```
"Server=server_name;Database=database_name;Trusted_Connection=True;TrustServerCertificate=True;"
```
*Lưu ý: Sửa `server_name` và `database_name` theo máy cá nhân ( database không nhất thiết phải có sẵn )

6. Mở project bằng Visual Studio, bật `PMC`, chạy lệnh sau:

```
Update-Database
``` 


## 2. Hướng dẫn set up môi trường làm việc cá nhân
Lưu ý: Nên hạn chế tối đa việc push thẳng vào nhánh main. 

Tạo nhánh cá nhân:

    git checkout -b <tên_nhánh_mới>
     
 Code...
 Lần đầu push:
 
    git add .
    git commit -m "nội_dung_muốn_commit"
    git push --set-upstream origin <tên_nhánh_mới>

## 3. Quy trình pull và push
Lưu ý: Chỉ đọc phần này sau khi đã thực hiện thành công phần 2. [tại đây](https://github.com/MoldBreaker/WinformFinal#2-h%C6%B0%E1%BB%9Bng-d%E1%BA%ABn-set-up-m%C3%B4i-tr%C6%B0%E1%BB%9Dng-l%C3%A0m-vi%E1%BB%87c-c%C3%A1-nh%C3%A2n)
<br>
Trước khi code, hãy luôn cập nhật code mới nhất từ `main` về

    git pull origin main
    
  Code...
  Trước khi push phải đảm bảo đang ở nhánh cá nhân:
  
    git add .
    git commit -m "nội_dung_muốn_commit"
    git push


<p align="center">Hết</p>

 



 

  
