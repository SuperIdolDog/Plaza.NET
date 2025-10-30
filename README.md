项目概述
Plaza 是一个基于 .NET 技术栈开发的应用程序，包含多个模块组件，涵盖后端服务、数据访问、模型定义、工具类库及前端相关资源，支持通过 Docker 容器化部署.

项目未完成(已死)，屎山一坨，越改需求越多，需求越多代码要改的就越多，项目的各种key需要自己加(微信小程序，腾讯地图，高德地图，支付宝沙箱等等)


技术栈：
后端框架：.NET 6.0、ASP.NET Core WebAPI、ASP.NET Core MVC

数据库：MySQL (原本是 SQL Server的,但是2+2服务器跑不动)

身份认证：Microsoft.Identity

ORM 及数据访问： EntityFrameworkCore及其相关工具(CodeFirst)

JSON 处理：Newtonsoft.Json

邮件处理：MailKit

API 文档： SwaggerUI(未写注释)

容器化：Docker、Docker Compose(配置部分未跑通，部署阿里云的时候不知道为啥连接不上数据库，疑是端口问题)

前端相关：Uni-App 框架及 uView UI 组件库(Vue2版本)

项目结构
核心模块：
Plaza.Net.Core：核心功能组件

Plaza.Net.Model：数据模型及 DTO 定义

Plaza.Net.Repository：数据访问层

Plaza.Net.Services：业务逻辑层

Plaza.Net.WebAPI：API 服务接口

Plaza.Net.MVCAdmin：后台管理系统（MVC）

Plaza.Net.Utility：通用工具类
<img width="2547" height="1527" alt="image" src="https://github.com/user-attachments/assets/ecc60472-9475-467f-b932-652b9588fffe" />
<img width="2512" height="1528" alt="image" src="https://github.com/user-attachments/assets/2a1d42fd-694c-4db9-983c-cbdb5f02e0c0" />

前端模块：
Plaza.Net.Uni-App：基于 Uni-App 的前端应用，使用 uView UI 组件库
<img width="661" height="1344" alt="image" src="https://github.com/user-attachments/assets/85059b70-c4c2-4636-9b3a-a1f0d832dcd1" />
<img width="683" height="1334" alt="image" src="https://github.com/user-attachments/assets/7f384db5-bdc1-4761-a7f6-a558255df0b0" />
<img width="623" height="1278" alt="image" src="https://github.com/user-attachments/assets/a59a9f22-0fe7-4145-ac9d-b14caf60bde7" />
<img width="620" height="1299" alt="image" src="https://github.com/user-attachments/assets/511aadfe-2589-4835-9306-b58e9c261c19" />
<img width="622" height="1314" alt="image" src="https://github.com/user-attachments/assets/37a11648-71c2-4805-bccb-342bb2993472" />
<img width="617" height="1307" alt="image" src="https://github.com/user-attachments/assets/35d40738-d3c6-4eed-940c-6869c6974512" />
<img width="636" height="1308" alt="image" src="https://github.com/user-attachments/assets/af3ee18d-5c30-4c70-931a-84acedb266c4" />
<img width="632" height="1293" alt="image" src="https://github.com/user-attachments/assets/a1d4b7c5-6827-4e34-90c8-ff1c21ea3fed" />
<img width="645" height="1322" alt="image" src="https://github.com/user-attachments/assets/f7d676e7-e474-4581-8e32-a0e69bc34bbc" />







