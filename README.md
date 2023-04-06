# E-commerce
.Net-Core-API E-commerce Project including Authentication, Authorizathion, Validation, REST APIS
- This Application structured based on Repository Design Pattern

# Technologies and Libraries
- .Net6 Core
- Entity Framework (EF) Core
- SQL Server
- JWT Authentication With Refresh Tokens

# REST APIS
- 	**Auth Route**
  	 
    **BaseURL** : https://localhost:7284/Auth
    | End Point | Method | Description | Body | Auth Header |
    | ----------- | ----------- | ----------- | ----------- | ----------- |
    | /SignUp | POST | Creating new accounts for users | name - email - password - role - profileImageUrl | --- |
    | /Login | POST | Login for users and admins | email - password | --- |

- 	**Users Route**
     	 
    **BaseURL** : https://localhost:7284/Users
    | End Point | Method |Description | Body | Auth Header |
    | ----------- | ----------- | ----------- | ----------- | ----------- |
    | /GetAllUsers | GET | For admins to get all users | --- | token : bearer (admin token) |
    | /GetUserById{id} | GET | For admins to get specific user by user-id | --- | token : Bearer (admin token) |
    | /EditUser | PUT | For user to edit his data | name - email - password - profileImageUrl | token : Bearer (user token) |
    | /DeleteUserById{id} | DELETE | For admins to delete specific user by user-id | --- | token : Bearer (admin token) |
 
 
 - 	**Products Route**
     	 
    **BaseURL** : https://localhost:7284/Products
    | End Point | Method |Description | Body | Auth Header |
    | ----------- | ----------- | ----------- | ----------- | ----------- |
    | /AddProduct | POST | For creating new Product by admin | name - descreption - price - imageUrl - categoryId - color - size | token : Bearer (admin token) |
    | /GetAllProducts | GET | For users to get products  | --- | --- |
    | /GetProductById{id} | GET | For users to find specific product by its id | --- | --- |
    | /EditProduct{id} | PUT | For updating Products by admin | name - descreption - price - imageUrl - categoryId - color - size | token : Bearer (admin token) |
    | /DeleteProductById{id} | DELETE | For admins to delete Products by product-id  | --- | token : Bearer (admin token) |
 
 
 - 	**CartItems Route**
     	 
    **BaseURL** : https://localhost:7284/CartItems
    | End Point | Method |Description | Body | Auth Header |
    | ----------- | ----------- | ----------- | ----------- | ----------- |
    | /AddCartItem | POST | For user to add product to his cart | productId - quantity | token : Bearer (user token) |
    | /EditCartItem{cartItemId}{quantity} | PUT | For updating cart-item by user | --- | token : Bearer (user token)
    | /DeleteCartItemById{cartItemId} | DELETE | For user to delete specific cart-item by cart-item-id from his cart | --- | token : Bearer (user token) |
   
   
- 	**Orders Route**
     	 
    **BaseURL** : https://localhost:7284/Orders
    | End Point | Method |Description | Body | Auth Header |
    | ----------- | ----------- | ----------- | ----------- | ----------- |
    | /AddOrder{orderAddress} | POST | For users to order his cart-items | --- | token : Bearer (user token) |
    | /GetOrderById{orderId} | GET | For user to get order by order-id | --- | token : Bearer (user token) |
    
    # Database
    - DataBase Schema 
      
      ![1](https://user-images.githubusercontent.com/58944474/229694215-0fff0cfc-31b0-4046-aa00-5a3ade9cbd11.png)
    
