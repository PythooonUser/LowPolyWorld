using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

	public static float RoundToFloat(float value, float delta) {
		/*
		int i = 0;
		float sign = Mathf.Sign (value);

		while (true) {
			if (delta * i >= Mathf.Abs (value)) {
				break;
			} else {
				i++;
			}
		}

		return delta * (i - 1) * sign;
		*/

		if (delta < 0.01f) {
			delta = 0.01f;
		}

		float ratio = value / delta;
		float ratioCeiled = Mathf.Ceil (ratio);

		if (1 - (ratioCeiled - ratio) >= 0.5f) {
			return ratioCeiled * delta;
		} else {
			return (ratioCeiled - 1) * delta;
		}
	}
}