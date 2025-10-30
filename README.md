项目概述
Plaza 是一个基于 .NET 技术栈开发的应用程序，包含多个模块组件，涵盖后端服务、数据访问、模型定义、工具类库及前端相关资源，支持通过 Docker 容器化部署
项目未完成已死，屎山一坨，越改需求越多，需求越多代码要改的就越多


技术栈：
后端框架：.NET 6.0、ASP.NET Core WebAPI、ASP.NET Core MVC
数据库：Microsoft SQL Server 2022
身份认证：Microsoft.Identity
ORM 及数据访问： EntityFrameworkCore及其相关工具(CodeFirst)
JSON 处理：Newtonsoft.Json
邮件处理：MailKit
API 文档： SwaggerUI(未写注释)
容器化：Docker、Docker Compose(配置部分未跑通，部署阿里云的适合不知道为啥连接不上数据库，疑是端口问题)
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
前端模块：
Plaza.Net.Uni-App：基于 Uni-App 的前端应用，使用 uView UI 组件库
