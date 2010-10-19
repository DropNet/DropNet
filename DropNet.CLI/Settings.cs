namespace DropNet.CLI
{
	internal class Settings
	{
		public string ApiKey { get; set; }
		public string Secret { get; set; }
		// public bool Sandboxed { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		
		public Settings()
		{
			ApiKey = "";
			Secret = "";
			// Sandboxed = false;
			Email = "";
			Password = "";
		}
	}
}