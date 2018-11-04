using UnityEngine;
using System.IO;
using System.Collections.Generic;

#pragma warning disable 0414
public class NativeShare
{
#if !UNITY_EDITOR && UNITY_ANDROID
	private static AndroidJavaClass m_ajc = null;
	private static AndroidJavaClass AJC
	{
		get
		{
			if( m_ajc == null )
				m_ajc = new AndroidJavaClass( "com.yasirkula.unity.NativeShare" );

			return m_ajc;
		}
	}

	private static AndroidJavaObject m_context = null;
	private static AndroidJavaObject Context
	{
		get
		{
			if( m_context == null )
			{
				using( AndroidJavaObject unityClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) )
				{
					m_context = unityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
				}
			}

			return m_context;
		}
	}
#elif !UNITY_EDITOR && UNITY_IOS
	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void _NativeShare_Share( string[] files, int filesCount, string subject, string text );
#endif

	private string subject;
	private string text;
	private string title;

	private string targetPackage;
	private string targetClass;

	private List<string> files;
	private List<string> mimes;

	public NativeShare()
	{
		subject = string.Empty;
		text = string.Empty;
		title = string.Empty;

		targetPackage = string.Empty;
		targetClass = string.Empty;

		files = new List<string>( 0 );
		mimes = new List<string>( 0 );
		Debug.Log("New Native share");
	}

	public NativeShare SetSubject( string subject )
	{
		if( string.IsNullOrEmpty(subject)){
			this.subject = subject;
			Debug.Log("Set subject: " + this.subject);
			}

		return this;
	}

	public NativeShare SetText( string text )
	{
		if( string.IsNullOrEmpty(text) ){
			this.text = text;
			Debug.Log("Set text: " + this.text);
		}
		return this;
	}

	public NativeShare SetTitle( string title )
	{
		if( string.IsNullOrEmpty(title)){
			this.title = title;
			Debug.Log("Set title: " + this.title);
		}
		return this;
	}

	public NativeShare SetTarget( string androidPackageName, string androidClassName = null )
	{
		if( !string.IsNullOrEmpty( androidPackageName ) )
		{
			targetPackage = androidPackageName;

			if( androidClassName != null )
				targetClass = androidClassName;
		}

		return this;
	}

	public NativeShare AddFile( string filePath, string mime = null )
	{
		if( !string.IsNullOrEmpty( filePath ) && File.Exists( filePath ) )
		{
			files.Add( filePath );
			mimes.Add( mime ?? string.Empty );
			Debug.Log("Files added");
		}
		else
			Debug.LogError( "File does not exist at path or permission denied: " + filePath );

		return this;
	}

	public void Share()
	{
		if( files.Count == 0 && subject.Length == 0 && text.Length == 0 )
		{
			Debug.LogWarning( "Share Error: attempting to share nothing!" );
			return;
		}

#if UNITY_EDITOR
		Debug.Log( "Shared!" );
#elif UNITY_ANDROID
		AJC.CallStatic( "Share", Context, targetPackage, targetClass, files.ToArray(), mimes.ToArray(), subject, text, title );
#elif UNITY_IOS
		_NativeShare_Share( files.ToArray(), files.Count, subject, text );
#else
		Debug.Log( "No sharing set up for this platform." );
#endif
	}

	#region Utility Functions
	public static bool TargetExists( string androidPackageName, string androidClassName = null )
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		if( string.IsNullOrEmpty( androidPackageName ) )
			return false;

		if( androidClassName == null )
			androidClassName = string.Empty;

		return AJC.CallStatic<bool>( "TargetExists", Context, androidPackageName, androidClassName );
#else
		return true;
#endif
	}

	public static bool FindTarget( out string androidPackageName, out string androidClassName, string packageNameRegex, string classNameRegex = null )
	{
		androidPackageName = null;
		androidClassName = null;

#if !UNITY_EDITOR && UNITY_ANDROID
		if( string.IsNullOrEmpty( packageNameRegex ) )
			return false;

		if( classNameRegex == null )
			classNameRegex = string.Empty;

		string result = AJC.CallStatic<string>( "FindMatchingTarget", Context, packageNameRegex, classNameRegex );
		if( string.IsNullOrEmpty( result ) )
			return false;

		int splitIndex = result.IndexOf( '>' );
		if( splitIndex <= 0 || splitIndex >= result.Length - 1 )
			return false;

		androidPackageName = result.Substring( 0, splitIndex );
		androidClassName = result.Substring( splitIndex + 1 );

		return true;
#else
		return false;
#endif
	}
	#endregion
}
#pragma warning restore 0414
