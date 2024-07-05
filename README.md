# ELW_DDD（英语学习网站项目）
  
## 项目启动配置
### 配置中心数据库（postgresql）  
执行sql命令生成 t_configs 表以及插入相关数据配置。
``` SQL
-- ----------------------------
-- Table structure for t_configs
-- ----------------------------
DROP TABLE IF EXISTS "public"."t_configs";
CREATE TABLE "public"."t_configs" (  
    "id" SERIAL PRIMARY KEY,  
    "name" VARCHAR(255) NOT NULL,  
    "value" VARCHAR(10000) NOT NULL  
);

-- ----------------------------
-- Records of t_configs
-- ----------------------------
INSERT INTO "public"."t_configs" VALUES (1, 'Cors', '{"Origins":["http://localhost:5173","http://localhost:5174"]}');
INSERT INTO "public"."t_configs" VALUES (2, 'FileService:SMB', '{"WorkingDir":"E:/ELW_DDD/Datas/SMB"}');
INSERT INTO "public"."t_configs" VALUES (3, 'FileService:Endpoint', '{"UrlRoot":"http://localhost/FileService"}');
INSERT INTO "public"."t_configs" VALUES (4, 'Redis', '{"ConnStr":"localhost"}');
INSERT INTO "public"."t_configs" VALUES (5, 'RabbitMQ', '{"HostName":"127.0.0.1","ExchangeName":"lwm_event_bus"}');
INSERT INTO "public"."t_configs" VALUES (6, 'ElasticSearch', '{"Url":"https://localhost:9200","UserName":"elastic","Password":"Q3IDAPdPmq60Vh*ygr7-","C_Fingerprint":"233558d64f1bc2f2569462dc62dc2d2bfaed3d80eb891966e98167f9edb9fa7e"}');
INSERT INTO "public"."t_configs" VALUES (7, 'JWT', '{"Issuer":"myIssuer","Audience":"myAudience","Key":"kj348@SKF2!$*U253jh","ExpireSeconds":"31536000"}');
INSERT INTO "public"."t_configs" VALUES (8, 'FileLogs', '{
"FileServiceLog":"",
"IdentityServiceLog":"",
"ListeningServiceAdminLog":"",
"ListeningServiceMainLog":"",
"MediaEncodingServiceLog":"",
"SearchServiceLog":""
}');
```
  
### 系统变量配置
添加【配置中心数据库】的连接字符串系统环境变量配置。
```
变量名：PostgreSqlDB:ConnStr
变量值：Server=127.0.0.1;Port=5432;Database=ELW_DDD;User ID=postgres;Password=MRlwm6499.;
```
  
### Nginx 配置
api网关（nginx）的相关配置。
``` nginx
worker_processes  1;

events {
    worker_connections  1024;
}

http {
    include       mime.types;
    default_type  application/octet-stream;

    sendfile        on;

    keepalive_timeout  65;

    server {
        listen       80;
        server_name  localhost;

        location /FileService/ {
			proxy_pass http://localhost:5291/;
			proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Real-PORT $remote_port;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto  $scheme;
            client_max_body_size 100m;
		}
        
        location /IdentityService/ {
			proxy_pass  http://localhost:5292/;
			proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Real-PORT $remote_port;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto  $scheme;
		}
		
		location /Listening.Admin/ {
			proxy_pass http://localhost:5293/;
			proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Real-PORT $remote_port;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto  $scheme;
			proxy_http_version 1.1;
			proxy_set_header Upgrade $http_upgrade;
			proxy_set_header Connection "upgrade";
		}

		location /Listening.Main/ {
			proxy_pass http://localhost:5294/;
			proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Real-PORT $remote_port;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;			
            proxy_set_header X-Forwarded-Proto  $scheme;
		}			
		
		location /MediaEncoder/ {
			proxy_pass http://localhost:5295/;
			proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Real-PORT $remote_port;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;	
            proxy_set_header X-Forwarded-Proto  $scheme;		
		}

		location /SearchService/ {
			proxy_pass http://localhost:5296/;
			proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Real-PORT $remote_port;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto  $scheme;
		}		

        error_page   500 502 503 504  /50x.html;
        location = /50x.html {
            root   html;
        }
    }
}
```
  
### 数据库迁移说明
```
Infrastructure 项目：（推荐）
1.设置为启动项目。
2.安装 Microsoft.EntityFrameworkCore.Tools 库。
3.编写 MyDesignTimeDbContextFactory 数据库迁移用到的脚本。

程序包管理器：
1.默认项目: Infrastructure。
2.执行迁移命令。
3.生成的迁移脚本在 Infrastructure 项目。
```
或者
```
WebAPI 项目：
1.设置为启动项目。
2.安装 Microsoft.EntityFrameworkCore.Tools 库。
3.编写 MyDesignTimeDbContextFactory 数据库迁移用到的脚本。

程序包管理器：
1.默认项目: Infrastructure（被启动项目WebAPI引用）
2.执行迁移命令。
3.生成的迁移脚本在 Infrastructure 项目。

注意：在这里需要先创建 "EWL_DDD" 数据库，然后配置完成 WebAPI 项目正常运行。
```
  
## FileService 项目
数据库迁移命令：
```
Add-Migration FileServiceInital -Context FSDbContext -OutputDir Pg_Migrations
Update-Database -Context FSDbContext
```
  
## IdentityService 项目
数据库迁移命令：
```
Add-Migration IdentityServiceInital -Context IdDbContext -OutputDir Pg_Migrations
Update-Database -Context IdDbContext
```
  
## ListeningService 项目
数据库迁移命令：
```
Add-Migration ListeningServiceInital -Context ListeningDbContext -OutputDir Pg_Migrations
Update-Database -Context ListeningDbContext
```
  
## MediaEncoderService 项目
数据库迁移命令：
```
Add-Migration MediaEncoderServiceInital -Context MediaEncoderDbContext -OutputDir Pg_Migrations
Update-Database -Context MediaEncoderDbContext
```