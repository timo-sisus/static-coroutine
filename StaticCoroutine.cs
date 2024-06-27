using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// Class that can be used to start coroutines from anywhere without needing a reference to a mono behaviour instance.
	/// </summary>
	[AddComponentMenu(Hidden)]
	public sealed class StaticCoroutine : MonoBehaviour
	{
		private const string Hidden = "";

		private static StaticCoroutine instance = null;
		private static MonoBehaviour monoBehaviour = null;

		/// <summary>
		/// Starts the given coroutine.
		/// </summary>
		/// <param name="coroutine"> Coroutine to start. </param>
		[return: MaybeNull]
		public static Coroutine Start([DisallowNull] IEnumerator coroutine) => MonoBehavior().StartCoroutine(coroutine);

		/// <summary>
		/// Stops the given coroutine.
		/// </summary>
		/// <param name="coroutine"> Coroutine to stop. </param>
		public static void Stop([DisallowNull] IEnumerator coroutine) => MonoBehavior().StopCoroutine(coroutine);

		/// <summary>
		/// Stops the given coroutine.
		/// </summary>
		/// <param name="coroutine"> Coroutine to stop. </param>
		public static void Stop([DisallowNull] Coroutine coroutine) => MonoBehavior().StopCoroutine(coroutine);

		/// <summary>
		/// Restarts the given coroutine.
		/// </summary>
		/// <param name="coroutine"> Coroutine to restart. </param>
		/// <param name="runningCoroutine">
		/// When this method is executed, this coroutine is stopped, if it is not null and currently running.
		/// When this method returns, contains a non-null, newly started coroutine.
		/// </param>
		public static void Restart(IEnumerator coroutine, ref Coroutine runningCoroutine)
		{
			if(runningCoroutine is not null)
			{
				Stop(runningCoroutine);
			}

			runningCoroutine = Start(coroutine);
		}

		/// <summary>
		/// Starts all the given coroutines.
		/// </summary>
		/// <param name="coroutines"> Zero or more coroutines to start. </param>
		/// <returns> A coroutine that finishes when all the coroutines given as arguments have finished. </returns>
		public static Coroutine StartAll(params IEnumerator[] coroutines) => MonoBehavior().StartCoroutine(instance.StartAllInternal(coroutines));

		private IEnumerator StartAllInternal(params IEnumerator[] coroutines)
		{
			int count = coroutines.Length;
			var runningCoroutines = new Coroutine[count];
			for(int i = 0; i < count; i++)
			{
				runningCoroutines[i] = StartCoroutine(coroutines[i]);
			}

			for(int i = 0; i < count; i++)
			{
				yield return runningCoroutines[i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static MonoBehaviour MonoBehavior()
		{
			return monoBehaviour ??= Initialize();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static MonoBehaviour Initialize()
			{
				if(!Application.isPlaying)
				{
					throw new NotSupportedException($"{nameof(StaticCoroutine)} cannot be used in edit mode.");
				}

				var gameObject = new GameObject(nameof(StaticCoroutine)) { hideFlags = HideFlags.HideInHierarchy };
				DontDestroyOnLoad(gameObject);
				instance = gameObject.AddComponent<StaticCoroutine>();
				return instance;
			}
		}
	}
}
