    function connectionMessageWrite(state, message) {
        $("#connectionMessage").find("div")[1].removeAttribute("class");
    $("#connectionMessage").find("div")[1].setAttribute("class", "alert alert-" + state);
    $("#connectionMessage").find("div")[1].innerHTML = message;
    $("#connectionMessage").show(2000);
            }
            $(document).ready(() => {
        $("#logOut").click(() => {
            localStorage.removeItem("accessToken");
            localStorage.removeItem("refreshToken");
        });

    // $(".toast").toast('show');

    var accessToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Imh1c2V5aW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJodXNleWluIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6Imh1c2V5aW4gaHVzZXlpbiIsImp0aSI6IjU1Zjk5YjQ2LTQwMzctNDNkOC1iOTBhLTQ2N2MxZWMxYTYwMyIsIm5iZiI6MTY1MTQ4NDM2MywiZXhwIjoxNjUxNTcwNzYzLCJpc3MiOiJodHRwczovL2dpdGh1Yi5jb20vaHVzZXlpbmFmc2luIiwiYXVkIjoiaHR0cHM6Ly9naXRodWIuY29tL2h1c2V5aW5hZnNpbiJ9.ePJa6-MHDUSuIls7phGmIeufyfDwIfrzHV1wAmKYNeDVSSvqSdRlngILaEgw4Ih1r105VC1pLtEmGtStaIBmjQ";
    const message = new signalR.HubConnectionBuilder()
    .withUrl("/chat",
    {accessTokenFactory: () => accessToken!= null ? localStorage.getItem("accessToken") : "" })
    .withAutomaticReconnect([10])
    .build();

    message.start()
                  .then(() => connectionMessageWrite("success", "Bağlantı başarılı."))
                  .catch(err => connectionMessageWrite("danger", "Bağlantı başarısız! Lütfen oturum açınız."));
               message.onclose((err) => $("#connectionMessage").hide(2000, () => {
        connectionMessageWrite("warning", "Bağlantı koptu!");
               }));

               $("#btnSendMessage").click(() => {
        message.invoke("SendMessage", $("#txtMessage").val());
    $("#txtMessage").val("");
               });
    let messageCount = 0;
               message.on("ReceiveMessage", message => {

        let object = '<div style="margin-top: 10px;" class="toast" id="toast-' + ++messageCount + '" role="alert" aria-live="assertive" aria-atomic="true" data-delay="5000">';
            object += ' <div class="toast-header">';
                object += '    <img src="messageicon.png" width="20px" class="rounded mr-2" alt="...">';
                    object += '    <strong class="mr-auto">Client</strong>';
                object += '    <small class="text-muted">Biraz önce</small>';
                object += '    <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">';
                    object += '       <span aria-hidden="true">&times;</span>';
                    object += '    </button>'
                object += ' </div>'
            object += ' <div class="toast-body">';
                object += message;
                object += ' </div>';
            object += '</div>';
        $("#divMessage").append(object);
        $("#toast-" + messageCount).toast('show');
               });

            });
