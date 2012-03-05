## .NET Client library for the Dropbox API

Full documentation here: http://dkdevelopment.net/what-im-doing/dropnet/

## How do I use it?

##### The Client:
To use DropNet you need an instance of the DropNetClient class, this class does everything for DropNet. This class takes the API Key and API Secret (These must be obtained from Dropbox to access the API).

```csharp
    _client = new DropNetClient("API KEY", "API SECRET");
```
 

##### Login/Tokens:
Dropbox now requires a web authentication to get a usable token/secret, so this is a 3 step process.

**Step 1.** Get Request Token - This step gets an oauth token from dropbox (NOTE: the token must pass the other steps before it can be used)

```csharp
    // Sync
    _client.GetToken();
    
    // Async
    _client.GetTokenAsync((userLogin) =>
        {
            //Dont really need to do anything with userLogin, DropNet takes care of it for now
        },
        (error) =>
        {
            //Handle error
        });
```

**Step 2.** Authorize App with Dropbox - This step involves sending the user to a login page on the dropbox site and having them authenticate there. The DropNet client has a function to return the url for you but the rest must be handled in app, this function also takes a callback url for redirecting the user to after they have logged in. (NOTE: The token still cant be used yet.)

```csharp
    var url = _client.BuildAuthorizeUrl();
    //Use the url in a browser so the user can login
```

Open a browser with the url returned by BuildAuthorizeUrl - After we have the authorize url we need to direct the user there (use some sort of browser here depending on the platform) and navigate the user to the url. This will prompt them to login and authorize your app with the API.

**Step 3.** Get an Access Token from the Request Token - This is the last stage of the process, converting the oauth request token into a usable dropbox API token. This function will use the clients stored Request Token but this can be overloaded if you need to specify a token to use.

```csharp
    // Sync
    var accessToken = _client.GetAccessToken(); //Store this token for "remember me" function
 
    // Async
    _client.GetAccessTokenAsync((accessToken) =>
        {
            //Store this token for "remember me" function
        },
        (error) =>
        {
            //Handle error
        });
```



**Best Practices:** Dropbox's Developer page states several times in bold red font that applications should not store a users Dropbox password and to help enforce this DropNet allows you to manually set a users Token and Secret on the client.

```csharp
    _client = new DropNetClient("API KEY", "API SECRET", "USER TOKEN", "USER SECRET");
    // OR
    _client = new DropNetClient("API KEY", "API SECRET");
    _client.UserLogin = new UserLogin { Token = "USER TOKEN", Secret = "USER SECRET" };
```

***

Want to chat about DropNet? http://jabbr.net/#/rooms/DropNet


***

 **Like DropNet?** Endore me on Coderwall
 
 [![endorse](http://api.coderwall.com/dkarzon/endorsecount.png)](http://coderwall.com/dkarzon)