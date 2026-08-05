using UnityEngine;
using System;

namespace SRDebugger.Services.Implementation
{
	public class ShakeDetector
	{
		public event Action OnShaked = null;

		private float accelerometerUpdateInterval = 1.0f / 60.0f;
		// The greater the value of LowPassKernelWidthInSeconds, the slower the
		// filtered value will converge towards current input sample (and vice versa).
		private float lowPassKernelWidthInSeconds = 1.0f;
		// This next parameter is initialized to 2.0 per Apple's recommendation,
		// or at least according to Brady! ;)
		private float shakeDetectionThreshold = 2.0f;

		private float lowPassFilterFactor = default;
		private Vector3 lowPassValue = default;

		public void Init()
		{
			lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
			shakeDetectionThreshold *= shakeDetectionThreshold;
			lowPassValue = Input.acceleration;
		}

		public void Refresh()
		{
			Vector3 acceleration = Input.acceleration;
			lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
			Vector3 deltaAcceleration = acceleration - lowPassValue;

			if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
			{
				OnShaked?.Invoke();
			}
		}
	}
}