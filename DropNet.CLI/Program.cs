using System;
using System.IO;
using Mono.Options;

namespace DropNet.CLI
{
	class Program
	{
		static Settings settings;

		static void ShowHelpAndExit(bool failexit)
		{
			Console.WriteLine("Usage: DropNet.CLI [OPTIONS] [METHOD] [METHOD ARGUMENTS]");
			Console.WriteLine("\tUploads a file to a dropbox account.");
			Console.WriteLine();
			Console.WriteLine("Examples:");
			Console.WriteLine("\tDropNet.CLI upload localfile.mp3 /music/");
			Console.WriteLine("\tDropNet.CLI upload localfile.mp3 /music/remotefile.mp3");
			Console.WriteLine("\tDropNet.CLI download /music/remotefile.mp3 ~/music");
			Console.WriteLine("\tDropNet.CLI authenticate");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("\t-e|--email       dropbox account");
			Console.WriteLine("\t-p|--password    dropbox password");
			Console.WriteLine("\t-a|--appkey      dropbox application key (if other than default)");
			Console.WriteLine("\t-s|--secret      dropbox application secret (if other than default)");
			// Console.WriteLine("\t-s|--sandbox     use dropbox sandbox");
			Console.WriteLine("\t-h|-?|--help     show this help");
			Console.WriteLine();
			Console.WriteLine();

			Environment.Exit(failexit ? 1 : 0);
		}

		static void ShowErrorHelpAndExit(string errormessage)
		{
			Console.WriteLine("Error: " + errormessage);
			Console.WriteLine();
			ShowHelpAndExit(true);
		}

		static void ShowErrorAndExit(string errormessage)
		{
			Console.WriteLine(errormessage);
			Console.WriteLine();
			Environment.Exit(1);
		}

		static void RequireUserInfo()
		{
			if (!string.IsNullOrEmpty(settings.Password) && !string.IsNullOrEmpty(settings.Email))
				return;

			ShowErrorHelpAndExit("Both email and password required.");
		}

		private static object doneSignal = false;

		static string CombineURL(string basepath, string path)
		{
			string ret = basepath;

			if (!string.IsNullOrEmpty(path))
			{
				if (!ret.EndsWith("/"))
					ret += "/";

				if (path.StartsWith("/"))
					ret += path.Substring(1);
				else
					ret += path;
			}

			return ret;
		}

		static string CombinePaths(string basepath, string path)
		{
			string ret = basepath;

			if (!string.IsNullOrEmpty(path))
			{
				if (!ret.EndsWith("" + Path.DirectorySeparatorChar))
					ret += Path.DirectorySeparatorChar;

				if (path.StartsWith(""+Path.DirectorySeparatorChar))
					ret += path.Substring(1);
				else
					ret += path;
			}

			return ret;
		}

		/*
		static void UploadDone(RestResponse resp)
		{
			Console.WriteLine("Upload done.");
			Console.WriteLine(resp.StatusCode);
			Console.WriteLine(resp.Content);
			// Console.WriteLine(resp.Request);
			
			lock (doneSignal)
			{
				Monitor.Pulse(doneSignal);
			}
		}
		*/
		/*
		static string GetFinalPath(string remotepath)
		{
			// remotepath = CombineURL(settings.Sandboxed ? "sandbox/" : "dropbox/", remotepath.Trim('/'));
			return remotepath;
		}
		*/
		static void Upload(string localfile, string remotepath)
		{
			if (!File.Exists(localfile))
				ShowErrorHelpAndExit(string.Format("Local file not found: {0}", localfile));

			var c = CreateAuthenticatedClient(false);

			var filename = Path.GetFileName(localfile);
			// remotepath = GetFinalPath(remotepath);
			if (!remotepath.EndsWith("/"))
			{
				filename = Path.GetFileName(remotepath);
				remotepath = Path.GetDirectoryName(remotepath);
			}
			
			Console.WriteLine("Uploading {0} to {1} as {2}...", localfile, remotepath, filename);

			byte[] data = null;
			try
			{
				data = File.ReadAllBytes(localfile);
			}
			catch (Exception z)
			{
				Console.WriteLine("Failed to read from file: "+localfile);
				Console.WriteLine(z);
			}
			
			if( data == null )
				return;
			
			Console.WriteLine("Read " + data.Length + " bytes");

			// Console.WriteLine("Final path: "+remotepath);

			var result = c.UploadFile(remotepath, filename, data);
			if (result)
			{
				Console.WriteLine("Upload successful.");
			}
			else
			{
				Console.WriteLine("Upload failed.");
			}

			/*
			doneSignal = new object();
			c.UploadFileAsync(remotepath, filename, data, UploadDone);
			lock (doneSignal)
			{
				Monitor.Wait(doneSignal);
			}*/
		}

		static void Download(string remotefile, string localpath)
		{
			if (!Directory.Exists(localpath))
				ShowErrorHelpAndExit(string.Format("Target directory not found: {0}", localpath));
			
			var c = CreateAuthenticatedClient(false);


			var filename = Path.GetFileName(remotefile);

			// var localfile = CombinePaths(localpath, filename);
			if (localpath.EndsWith("" + Path.DirectorySeparatorChar))
				localpath = CombinePaths(localpath, filename);

			Console.WriteLine("Downloading {0} to {1}...", remotefile, localpath);

			byte[] data = null;
			try
			{
				data = c.GetFile(remotefile);
			}
			catch (Exception z)
			{
				Console.WriteLine("Download failed:");
				Console.WriteLine(z);
			}

			if( data == null )
				return;
			
			Console.WriteLine("Got " + data.Length + " bytes");

			try
			{
				File.WriteAllBytes(localpath, data);
				Console.WriteLine("Download successful.");

			}
			catch (Exception w)
			{
				Console.WriteLine("Failed to write to file: "+localpath);
				Console.WriteLine(w);
			}
		}

		private static void Authenticate()
		{
			var c = CreateAuthenticatedClient(true);
			Console.WriteLine("Authenticated as {0}", c.Account_Info().display_name);
		}

		private static DropNetClient CreateAuthenticatedClient(bool showsuccessfulinfo)
		{
			RequireUserInfo();

			var c = new DropNetClient(settings.ApiKey, settings.Secret);
			try
			{
				var ul = c.Login(settings.Email, settings.Password);
				if (!string.IsNullOrEmpty(ul.Token))
				{	
					if (showsuccessfulinfo)
					{
						Console.WriteLine("Authentication successful.");
						Console.WriteLine("\tToken: " + ul.Token);
						Console.WriteLine("\tSecret: " + ul.Secret);
					}
					else
					{
						Console.WriteLine("Authenticated.");
					}
				}
				else
				{
					Console.WriteLine("Authentication failed.");
				}
			}
			catch (Exceptions.DropboxException e)
			{
				Console.WriteLine("Authentication failed.");
				Console.WriteLine(e);
			}
			return c;
		}

		static void Main(string[] args)
		{
			settings = new Settings();
			settings.ApiKey = ApplicationDefaults.AppKey;
			settings.Secret = ApplicationDefaults.AppSecret;

			var showhelpflag = false;

			var opts = new OptionSet {
				{ "v|verbose", v => { } },
				{ "h|?|help", v => { showhelpflag = v != null; } },
				{ "e|email=", v => { settings.Email = v; } },
				{ "p|password=", v => { settings.Password = v; } },
				{ "a|appkey=", v => { settings.ApiKey = v; } },
				{ "s|secret=", v => { settings.Secret = v; } },
				// { "s|sandbox", v => { settings.Sandboxed = true; } }
			};

			var extra = opts.Parse(args);

			if (showhelpflag)
				ShowHelpAndExit(false);

			if (extra.Count == 0)
				ShowErrorHelpAndExit("Nothing to do, no method specified.");

			string method = extra[0].ToLower().Trim();

			if (method == "upload" || method == "up")
			{
				if (extra.Count != 3)
					ShowErrorHelpAndExit(string.Format("Wrong number of arguments to upload method, {0} specified, {1} required", extra.Count - 1, 2));

				Upload(extra[1], extra[2]);
			}
			else if (method == "download" || method == "down")
			{
				if (extra.Count != 3)
					ShowErrorHelpAndExit(string.Format("Wrong number of arguments to download method, {0} specified, {1} required", extra.Count - 1, 2));

				Download(extra[1], extra[2]);
			}
			else if (method == "authenticate")
			{
				Authenticate();
			}
			else
				ShowErrorHelpAndExit(string.Format("Unknown method specified: {0}", method));
		}

	}
}
