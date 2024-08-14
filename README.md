Documentation
  Project Architecture with N-Tier Architecure .Net 7
  
    This project include Postman collection and database script
  
    For login System
        Email : mahmoud@domain.com
        Password : MAHMOUD@123
        
    Setup Instructions:
        # Prerequisites:
            software required (e.g., .NET 7 SDK version, SQL Server 20 ).
            dependencies that need to be installed
            * WebApi Layer  
              (Microsoft.AspNetCore.Authentication.JwtBearer ,
               Microsoft.AspNetCore.OpenApi ,
               Microsoft.EntityFrameworkCore.Design ,
               Microsoft.IdentityModel.Tokens ,
               System.IdentityModel.Tokens.Jwt).
               
           * BusinessLogic Layer
              (AutoMapper, Microsoft.AspNetCore.Identity).
              
           * Data Access Layer
              (Microsoft.AspNetCore.Identity.EntityFrameworkCore,
              Microsoft.EntityFrameworkCore.SqlServer,
              Microsoft.EntityFrameworkCore.Tools)

          * Tests (MSTest .Net 7)
             (AutoFixture , Moq , FluentAssertions , Microsoft.NET.Test.Sdk , MSTest.TestAdapter, MSTest.TestFramework).
             
      # Configuration
         appSetting connection string 
             "ConnectionStrings": {
                                    "conn": "server =.; database=NewsApplicationDB; integrated security=true; Encrypt=false;"
                                  }
     # Database Setup:
        apply migration over database (update-database) over DAL

     # Tools:
         Use tools like Swagger to automatically generate API documentation.
