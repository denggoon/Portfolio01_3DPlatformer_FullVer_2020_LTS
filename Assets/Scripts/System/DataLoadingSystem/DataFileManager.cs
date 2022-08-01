using UnityEngine;
using System.Collections;

public enum FILE_LOAD_MODE
{
	OFFLINE = 0,
	ONLINE,
}

public class DataFileManager : MonoBehaviour {

	private static DataFileManager instance_;

	public static DataFileManager instance
	{
		get
		{
			return instance_;
		}
	}

	public FILE_LOAD_MODE eFileLoadMode = FILE_LOAD_MODE.ONLINE;

	void Awake()
	{
		instance_ = this;

		PlayerPrefs.DeleteKey ("FileLoadMode");

		PlayerPrefs.SetInt("FileLoadMode", System.Convert.ToInt32(eFileLoadMode));

		DontDestroyOnLoad (this.gameObject);
	}

	void OnDestroy()
	{
		instance_ = null;
	}


}
