using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Wrapper around Unity's WWW request class.
/// </summary>
public class WWWRequest : IEnumerator
{
	/// <summary>
	/// The default timeout for requests (in seconds).
	/// </summary>
	private const float DEFAULT_TIMEOUT_SECONDS = 30f;

	private UnityWebRequest wwwRequest;
	private float timeout;
	private float requestDuration;

	/// <summary>
	/// Gets a value indicating whether this ongoing WWW request completed successfully.
	/// This should can be tested after yielding on this instance to check the request completed successfully.
	/// </summary>
	public bool Completed
	{
		get { return isDone && string.IsNullOrEmpty(wwwRequest.error); }
	}

	public string url
	{
		get { return wwwRequest.url; }
	}

	public string error
	{
		get { return wwwRequest.error; }
	}

	/// <summary>
	/// Gets a value indicating whether this request is done or not.
	/// </summary>
	public bool isDone
	{
		get { return wwwRequest.isDone; }
	}

	public string text
	{
		get { return Completed ? wwwRequest.downloadHandler.text : null; }
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnityWebRequest"/> class.
	/// </summary>
	public WWWRequest(UnityWebRequest wwwRequest)
		: this(wwwRequest, DEFAULT_TIMEOUT_SECONDS)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnityWebRequest"/> class.
	/// </summary>
	public WWWRequest(UnityWebRequest wwwRequest, float timeout)
	{
		if (wwwRequest == null)
		{
			throw new System.ArgumentNullException("wwwRequest");
		}

		this.wwwRequest = wwwRequest;
		this.timeout = timeout;
	}

	#region IEnumerator implementation

	public bool MoveNext ()
	{
		requestDuration += Time.deltaTime;

		return !wwwRequest.isDone && requestDuration <= timeout;
	}

	public void Reset ()
	{
		wwwRequest = null;
		requestDuration = 0f;
		timeout = 0f;
	}

	public object Current 
	{
		get
		{
			return null;
		}
	}

	#endregion
}