
using System.Collections;
using Google.Play.Review;
using UnityEngine;

public class InAppReviewManager : Singleton<InAppReviewManager>
{
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;

    private bool _isRequested;
    private bool _isRequestCompleted;
    
    public void Request()
    {
        _reviewManager = new ReviewManager();
        _playReviewInfo = null;
        _isRequestCompleted = false;
        _isRequested = true;
        StartCoroutine(RequestCoroutine());
    }

    public void LaunchReviewFlow()
    {
        if (_isRequested)
            StartCoroutine(LaunchReview());
    }

    IEnumerator RequestCoroutine()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        
        _isRequestCompleted = true;
        
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogWarning($"Request Fail: {requestFlowOperation.Error}");
            yield break;
        }
        
        _playReviewInfo = requestFlowOperation.GetResult();
    }

    IEnumerator LaunchReview()
    {
        while (!_isRequestCompleted)
            yield return null;
        
        if (_playReviewInfo == null)
            yield break;
        
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogWarning($"Request Fail: {launchFlowOperation.Error}");
            yield break;
        }
    }
}