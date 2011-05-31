.NET Client library for the Dropbox API

http://dkdevelopment.net/what-im-doing/dropnet/

Example usage:

	var dropNetclient = new DropNetClient("API_KEY", "API_SECRET");
 
	//call the functions you want from the client
	dropNetclient.Login("test@example.com", "password");

	var rootDetails = dropNetclient.GetMetaData();
	//rootDetails.Contents is a list of the files/folders in the root