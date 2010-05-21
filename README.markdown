.NET Client library for the Dropbox API

<a href="http://dkdevelopment.net/">http://dkdevelopment.net/</a> //TODO - Update

Example usage:

	var dropNetclient = new DropNetClient("API_KEY", "API_SECRET");
 
	//call the functions you want from the client
	dropNetclient.Login("test@example.com", "password");

	var rootDetails = dropNetclient.GetMetaData("/");
	//rootDetails.Contents is a list of the files/folders in the root