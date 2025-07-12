using System;

public interface ILoader
{

	BundleRes LoadSync(string bundleName);

	void LoadAsync(string bundleName, Action<BundleRes> onLoaded);


}
