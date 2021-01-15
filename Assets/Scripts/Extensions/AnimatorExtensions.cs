using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// Extension methods for the Animator class.
/// </summary>
public static class AnimatorExtensions
{
	/// <summary>
    /// Wait for the animation to end
    /// </summary>
    /// <param name="animator"> the animator </param>
    /// <param name="animationName"> animation name </param>
    public static IEnumerator WaitAnimationEnd(this Animator animator, string animationName)
    {
	    var animationLength = animator.AnimationLength(animationName);
		
	    yield return new WaitForSeconds(animationLength);
		
	    if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
	    {
		    var normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

		    // Check if the animation has ended (normalizedTime >= 1)
		    if (normalizedTime < 1f && normalizedTime > 0f)
		    {
			    // If the normalizedTime is smaller than 1 that means that the animation hasn't ended yet and we need to wait the extra delta time
			    yield return new WaitForSeconds(animationLength * (1f - normalizedTime));
		    }
	    }
    }
	
	/// <summary>
	/// Returns the given animation's length (or 0, if no clip with the given name was not found).
	/// </summary>
	private static float AnimationLength(this Animator animator, string animationName)
	{
		RuntimeAnimatorController runTimeAnimatorController = animator.runtimeAnimatorController;
		var animationClip = runTimeAnimatorController.animationClips.FirstOrDefault (clip => clip.name == animationName);
		
		return animationClip && animationClip != null ? animationClip.length : 0f;
	}
}