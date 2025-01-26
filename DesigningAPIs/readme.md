- start with empty and separate projects.
- focus on products and provide seed with db data and everything but an empty controller, services and repository


- present rest, what is
- talk about HTTP REST contraints and all
- - status codes
- how to design the api endpoints


____________________
- implement the REST endpoint for Products toghether
- document the endpoints with Produces

`
[ProducesResponseType(204)]
[ProducesResponseType(400)]
[ProducesResponseType(404)]
`
- then add ProductReviews
- then add Product Categories
- then add Product Images - hard to implement
- then add Product Tags - seems a good idea
- then add Product Variants

-----------

- implement the services and repositories for the products
- add caching to the products Repo - in memory JPrince
- maybe add another variant with RedisCache or talk about it
- emphasize that we can filter products based on REST spects on query string. Modify parameters and add IQueryable to the repo for the	
GetAll method
- add your Own ProblemDetails 

- Test everything: https://learn.microsoft.com/en-us/aspnet/core/test/http-files?view=aspnetcore-8.0

- come back to slides and show how important is the Accept-header, and how to implement it in the controller											
by adding XML output formatter and test it again.



Customer 
customerID": "ABCXY",
  "companyName": "ABC Corp",
  "contactName": "John Smith",
  "contactTitle": "Sir",
  "address": "Main Street",
  "city": "New York",
  "region": "NY",
  "postalCode": "90210",
  "country":  "USA",
  "phone": "(123) 555-1234"


- implement the Customer endpoint
- implement the services and repositories for the customers
- add caching to the customers Repo - in memory JPrince
- start adding library that obfuscated the ids in the browser
- 

Log levels can be set for the namespace in which the functionality is defined. Nested namespaces allow us to control which functionality has logging enabled:

Microsoft: Include all log types in the Microsoft namespace.
Microsoft.AspNetCore: Include all log types in the Microsoft.AspNetCore namespace.
Microsoft.AspNetCore.HttpLogging: Include all log types in the Microsoft.AspNetCore. HttpLogging namespace.
Let�s see HTTP logging in action:

- adding Odata
   https://github.com/markjprice/apps-services-net8/blob/main/docs/ch08-odata.md
- adding GraphQL

---------------------

https://github.com/GaProgMan/OwaspHeaders.Core/blob/main/README.md


= add a http client in the mvc ui project
builder.Services.AddHttpClient(name: "Northwind.WebApi",
  configureClient: options =>
  {
    options.BaseAddress = new Uri("https://localhost:5151/");
    options.DefaultRequestHeaders.Accept.Add(
      new MediaTypeWithQualityHeaderValue(
      mediaType: "application/json", quality: 1.0));
  });