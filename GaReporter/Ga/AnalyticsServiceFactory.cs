using System;
using Google.Apis.Analytics.v3;
using Google.Apis.Services;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;

namespace Tools
{
	public static class AnalyticsServiceFactory
	{
		public static AnalyticsService GetService(string keyPath, string accountEmailAddress, string applicationName)
		{
			return new AnalyticsService(new BaseClientService.Initializer() {
				HttpClientInitializer = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(accountEmailAddress) {
					Scopes = new[] {
						AnalyticsService.Scope.AnalyticsReadonly
					}
				}.FromCertificate(new X509Certificate2(keyPath, "notasecret", X509KeyStorageFlags.Exportable))),
				ApplicationName = applicationName
			});
		}
	}
}