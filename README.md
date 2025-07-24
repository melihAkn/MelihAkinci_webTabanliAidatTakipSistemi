
<h2 align="center"> <a href="#tr">Turkish</a> | <a href="#eng">English</a>  </h2>

<br>
<h1 id="tr" align="center">Web tabanlı aidat takip otomasyonu</h1>

apartman veya site yöneticilerinin aidat takibini yapabileceği, kullanıcıların ise borç ve ödeme bilgilerini görüntüleyebileceği web tabanlı bir sistem.

## projenin kurulumu ve çalıştırılması

önce projeyi bu komut ile klonlayalım.

```shell
git clone https://github.com/melihAkn/MelihAkinci_webTabanliAidatTakipSistemi.git
```
önce backend'i çalıştıralım

1. sonra proje dizinine bu komutla girelim

```shell
cd .\MelihAkıncı_webTabanliAidatTakipSistemi\
```

2. proje dizinin de .env adında bir klasör oluşturalım ve için bu değişkenleri ekleyelim
	bağlantı dizesin de ki şifreyi mysql programından alıp yazalım.
	mailin kullanıcıların mail adresine gönderilmesi için gmail üzerinden app id alınması gerekiyor. sonra mail adresi ve app şifresini yazalım.(opsiyonel)
	
```shell 

MYSQL_CONNECTION_STRING=server=localhost;port=3306;database=webTabanliAidatYonetimSistemi;user=root;password=şifreniz;

JWT_KEY=webtabanliAidatTakipSistemiGizliAnahtar
JWT_ISSUER=webTabanliAidatTakipSistemi
JWT_AUDIENCE=Users
JWT_EXPIRES_MINUTES=240.0

// mail gönderimi için gerekli değişkenler(opsiyonel)
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_CREDENTIAL_MAIL_ADDRESS=mail adresiniz
EMAIL_CREDENTIAL_PASSWORD= app şifreniz
EMAIL_SENDER_NAME=Web Tabanlı Aidat Yönetim Sistemi

```



3. sonra migration'u çalıştıralım

```shell
dotnet ef database update
```

4. sonra backend'i çalıştıralım

```shell
dotnet run
```

5. frontend in dosya dizinine gidelim

```shell
cd /Frontend
```

6. sonra frontend'i çalıştıralım.
```shell
npm run dev
```

7.son olarak bu adrese giderek projeyi görebiliriz.

http://localhost5173



Tüm API uç noktalarının detaylı dökümantasyonu Swagger UI üzerinden erişilebilir:

🔗 http://localhost:5263/swagger


<h1 id="eng" align="center">Web based apartment maintenance fee system</h1>

A web-based system where apartment or site managers can track maintenance fees, and users can view their debts and payment information.

## Installing and Running the Project

First, let's clone the project:

```shell
git clone https://github.com/melihAkn/MelihAkinci_webTabanliAidatTakipSistemi.git
```
and let's start backend.

1. Navigate to the backend project directory:

```shell
cd .\MelihAkıncı_webTabanliAidatTakipSistemi\
```

2. Let's create a file named .env in the project directory and add the following variables inside it.
Get the password in the connection string from the MySQL program and enter it accordingly.
To send emails to users' email addresses, you need to get an app password from Gmail. Then, add your email address and the app password to the environment file. (This step is optional.)
	
```shell

MYSQL_CONNECTION_STRING=server=localhost;port=3306;database=webTabanliAidatYonetimSistemi;user=root;password=şifreniz;

JWT_KEY=webtabanliAidatTakipSistemiGizliAnahtar
JWT_ISSUER=webTabanliAidatTakipSistemi
JWT_AUDIENCE=Users
JWT_EXPIRES_MINUTES=240.0

EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_CREDENTIAL_MAIL_ADDRESS=mail adresiniz
EMAIL_CREDENTIAL_PASSWORD= app şifreniz
EMAIL_SENDER_NAME=Web Tabanlı Aidat Yönetim Sistemi

```
3. Apply the latest database migrations:

```shell
dotnet ef database update
```

4. Run the backend server:

```shell
dotnet run
```

5. Navigate to the frontend directory:

```shell
cd /Frontend
```

6. Start the frontend development server:
```shell
npm run dev
```

7.and we can view the project at this address:

http://localhost5173

All API endpoints can be viewed and tested via Swagger UI:
http://localhost:5263/swagger


