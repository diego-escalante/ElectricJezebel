using UnityEngine;
using System.Collections;

public class retainAspectRatio : MonoBehaviour {

  private float ratio = 4f/3f;
  private Camera bgCam, mainCam;

  private void Start() {
    bgCam = transform.Find("Back Camera").GetComponent<Camera>();
    mainCam = GetComponent<Camera>();
  }

  private void Update() {
    CalculateMainCameraDimensions();
  }

  public void CalculateMainCameraDimensions() {
    if (bgCam.aspect < ratio) {
      mainCam.rect = new Rect(0f, (1.0f - bgCam.aspect / ratio) / 2.0f, 1.0f, bgCam.aspect / ratio);
    }
    else {
      mainCam.rect = new Rect((1.0f - ratio / bgCam.aspect) / 2.0f, 0, ratio / bgCam.aspect, 1.0f);
    }
  }
}
