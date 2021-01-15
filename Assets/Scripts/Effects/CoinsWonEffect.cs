using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinsWonEffect : MonoBehaviour {

	private const string resourceName = "coinsWonEffect";

	public Transform Container;
	public ParticleSystem coinsForSlot;
	public int maxParticles;
	public int numberOfAttackCoins;

	/// <summary>
	/// Cached reference of the GameObject for the coins effect.
	/// </summary>
	private static List<CoinsWonEffect> _coinsWonEffects;

	private static CoinsWonEffect _currEffect;
 
    public static float ActivateEffect(float percentOfParticels, float x = 0, float y = 0)
	{
		// Only instantiate this object once.
		if (_coinsWonEffects == null || _coinsWonEffects[0] == null)
		{
			_coinsWonEffects = new List<CoinsWonEffect>();
		//	var resource = Resources.Load(GameData.effectsDirectory + "/" + resourceName) as GameObject;
	//		CreateEffects(2, resource);
			_currEffect = _coinsWonEffects[0];
		}
		else
		{
			_currEffect = _coinsWonEffects[0].coinsForSlot.isPlaying ? _coinsWonEffects[1] : _coinsWonEffects[0];
		}
		
		_currEffect.Activate(percentOfParticels,2.5f);

		_currEffect.Container.position = new Vector3(x, y, _currEffect.Container.position.z);

		return 2.5f; // TEMP TIME
	}

	private void Activate(float percentOfParticels, float activeTime)
	{
		if (coinsForSlot != null)
		{
			coinsForSlot.gameObject.SetActive(true);
			var particleEmission = coinsForSlot.emission;
			particleEmission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
			coinsForSlot.Emit(Mathf.RoundToInt(maxParticles*percentOfParticels));
		}

		StartCoroutine(Deactivate(activeTime));
	}

	private IEnumerator Deactivate(float activeTime)
	{
		yield return new WaitForSeconds(activeTime);
		coinsForSlot.gameObject.SetActive(false);
	}

	private static void CreateEffects(int amount, GameObject resource)
	{
		for (int i = 0; i < amount; i++)
		{
			var currParent = Instantiate(resource, Vector3.zero, Quaternion.identity).transform;
			var currEffect = currParent.GetComponent<CoinsWonEffect>();
			currEffect.Container = currParent;
			currParent.name = "CoinsPopEffect" + i;
			_coinsWonEffects.Add(currEffect);
		}
	}

}