
# Giriş

Bu program websocket teknolojisi kullanılarak kullanıcıların anlık mejajlaşmasını sağlayan bir WebAPI programıdır.


## Kullanımı
  ### Main route bu şekildedir.
  `[site_url]/api/[hub_name]/[action]`
   

    ### Auth Hub
      Register: `wss://localhost:7149/auth/register`
      Register: `wss://localhost:7149/auth/login`
      
    ### Chat Hub
      SendUsers: `wss://localhost:7149/chat/sendUsers`
      SendPrivateMessage: `wss://localhost:7149/chat/sendUsers`
      SendRoomMessage: `wss://localhost:7149/chat/sendUsers`
      AddRoom: `wss://localhost:7149/chat/sendUsers`


  
    
  #### Technologies
- .NET Core 6.13
- SignalR (WebSocket)
- MsSql
- Entity Framework Core
- Automapper

#### Techniques
- JWT (Json Web Tokens)
- IoC 
- Microsoft Built In Dependency Resolver



#
