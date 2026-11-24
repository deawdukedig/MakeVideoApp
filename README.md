# MakeVideoApp (MP3 + Image to Full HD Video)

แอปพลิเคชัน Windows Desktop (C# WinForms) แบบ Offline สำหรับรวมภาพนิ่งและไฟล์เสียงให้เป็นวิดีโอความละเอียด **Full HD (1920x1080)** เพื่ออัปโหลดลง YouTube โดยทำงานผ่าน FFmpeg ในเครื่อง 100%

## Features
* **Full HD 1080p Output:** แปลงภาพนิ่งและเสียงเป็นไฟล์ `.mp4` (H.264 + AAC 192kbps) ขนาด 1920x1080 อัตโนมัติ พร้อมรักษาสัดส่วนภาพเดิมและเติมขอบดำ (Letterbox/Pillarbox) ไม่ให้ภาพยืดหรือบิดเบี้ยว
* **Large & Accessible UI:** ออกแบบขนาดตัวอักษรใหญ่ (14pt–16pt) อ่านง่าย สบายตา
* **Smart Drag & Drop:** ลากไฟล์ภาพ (`.jpg`, `.png`, `.bmp`) และไฟล์เสียง (`.mp3`, `.wav`, `.m4a`, `.aac`) มาวางตรงไหนก็ได้ในหน้าต่างโปรแกรม ระบบจะแยกประเภทไฟล์ลงช่องให้อัตโนมัติ
* **16:9 Thumbnail Preview:** แสดงตัวอย่างภาพในกรอบสัดส่วน 16:9 ทันทีที่เลือกไฟล์ (ไม่ล็อกไฟล์ต้นฉบับ)
* **Auto Output Naming:** ตั้งชื่อไฟล์ปลายทาง `.mp4` ตามชื่อไฟล์เสียงให้อัตโนมัติ
* **Auto Open Explorer & Clear:** เปิดโฟลเดอร์และไฮไลต์ไฟล์วิดีโอทันทีเมื่อแปลงเสร็จ พร้อมปุ่มล้างค่า (Clear) เพื่อเริ่มงานถัดไปได้รวดเร็ว

---

## Requirements
* **OS:** Windows 10 หรือ Windows 11 (64-bit)
* **Runtime / SDK:** [.NET 8.0 SDK](https://microsoft.com) (สำหรับ Build และรันโปรแกรม)
* **FFmpeg:** ต้องติดตั้ง FFmpeg ในเครื่อง หรือวางไฟล์ `ffmpeg.exe` ไว้ในโฟลเดอร์เดียวกับตัวโปรแกรม

---

## Installation (ติดตั้ง FFmpeg ด้วย Winget)

สามารถติดตั้ง FFmpeg และตั้งค่า System PATH อัตโนมัติได้ในคำสั่งเดียวผ่าน Command Prompt (CMD) หรือ PowerShell:

```cmd
winget install -e --id Gyan.FFmpeg
```

หลังติดตั้งเสร็จ ให้ **ปิดหน้าต่าง CMD/PowerShell แล้วเปิดใหม่** จากนั้นตรวจสอบว่าติดตั้งสำเร็จด้วยคำสั่ง:

```cmd
ffmpeg -version
```

*(หมายเหตุ: หากไม่ต้องการติดตั้งลงเครื่อง สามารถดาวน์โหลดไฟล์ `ffmpeg.exe` มาวางไว้ในโฟลเดอร์เดียวกับ `MakeVideoApp.exe` ได้เช่นกัน)*

---

## Build Instructions

เปิด Command Prompt (CMD) หรือ PowerShell ในโฟลเดอร์โปรเจกต์ แล้วเลือกวิธี Build ตามต้องการ:

### วิธีที่ 1: รันสคริปต์อัตโนมัติ
ดับเบิลคลิกไฟล์ `build.bat` หรือพิมพ์คำสั่ง:
```cmd
build.bat
```

### วิธีที่ 2: Build ผ่านคำสั่ง `dotnet publish`
สร้างไฟล์ `.exe` แบบไฟล์เดียว (Single File) ลงในโฟลเดอร์ `output`:
```cmd
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./output
```

หลัง Build เสร็จ ไฟล์โปรแกรมจะอยู่ที่ `output\MakeVideoApp.exe` สามารถดับเบิลคลิกเปิดใช้งานได้ทันที

