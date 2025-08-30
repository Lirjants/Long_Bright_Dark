using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
    

public class Textbox : MonoBehaviour
{
    [Header("Text Settings")]
    [Tooltip("If left empty, this component will be fetched from the same GameObject.")]
    public TextMeshProUGUI textComponent;

    [Tooltip("Default text for testing; you can call DisplayText() at runtime to show new text.")]
    [TextArea(5, 10)]
    public string fullText = "";
    [Header("Typing Speed")]
    [Tooltip("Delay in seconds between each word.")]
    public float delayBetweenWords = 0.05f;

    [Header("Pagination")]
    [Tooltip("Shown when waiting for user input to continue (e.g., an arrow icon).")]
    public GameObject continueIndicator;

    // Internal state
    private readonly List<string> pages = new List<string>();
    private Coroutine displayCoroutine;
    private bool isTyping = false;
    private bool skipTyping = false;
    private bool isWaitingForUserInput = false;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        if (textComponent != null)
            textComponent.enableWordWrapping = true; // ensure wrapping is on for measurement
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(fullText))
            DisplayText(fullText);
    }

    void OnDisable()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }
        isTyping = false;
        skipTyping = false;
        isWaitingForUserInput = false;

        if (continueIndicator != null)
            continueIndicator.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Skip the typing animation
                skipTyping = true;
            }
            else if (isWaitingForUserInput)
            {
                // Advance to the next page
                isWaitingForUserInput = false;
            }
        }
    }

    /// <summary>
    /// Displays a new message, stopping any previous one.
    /// </summary>
    public void DisplayText(string textToDisplay)
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        displayCoroutine = StartCoroutine(ShowTextCoroutine(textToDisplay));
    }

    /// <summary>
    /// Master coroutine: waits for layout, paginates, then types out each page.
    /// </summary>
    private IEnumerator ShowTextCoroutine(string text)
    {
        // Let layout settle so rect sizes are valid (prevents tiny/zero rect height causing 2–3 words/page).
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        if (textComponent != null)
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(textComponent.rectTransform);

        PaginateText(text);

        for (int i = 0; i < pages.Count; i++)
        {
            string page = pages[i];
            yield return StartCoroutine(TypeOutPage(page));

            // Avoid the same click that skipped typing also advancing the page
            yield return null;

            // Wait for input unless it's the last page
            if (i < pages.Count - 1)
            {
                isWaitingForUserInput = true;
                if (continueIndicator != null)
                    continueIndicator.SetActive(true);

                yield return new WaitUntil(() => !isWaitingForUserInput);

                if (continueIndicator != null)
                    continueIndicator.SetActive(false);
            }
        }

        // Optional: clear after finishing
        // if (textComponent != null) textComponent.text = "";
    }

    /// <summary>
    /// Types out a single page word-by-word. Click to skip.
    /// </summary>
    private IEnumerator TypeOutPage(string page)
    {
        if (textComponent == null)
            yield break;

        isTyping = true;
        skipTyping = false;
        textComponent.text = "";

        string[] words = page.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            textComponent.text += words[i] + " ";

            if (skipTyping)
                break;

            if (delayBetweenWords > 0f)
                yield return new WaitForSeconds(delayBetweenWords);
            else
                yield return null; // at least one frame so input can be read
        }

        // Ensure the full page is visible after typing or skip
        textComponent.text = page;

        skipTyping = false;
        isTyping = false;
    }

    /// <summary>
    /// Splits the input string into pages that fit the TextMeshProUGUI rect (accounting for margins).
    /// </summary>
    private void PaginateText(string text)
    {
        pages.Clear();

        if (textComponent == null)
            return;

        string[] words = text.Split(' ');
        if (words.Length == 0)
            return;

        // Real rect AFTER layout; subtract TMP margins
        var rect = textComponent.rectTransform.rect;
        var m = textComponent.margin; // x=left, y=top, z=right, w=bottom

        float availableWidth  = Mathf.Max(1f, rect.width  - (m.x + m.z));
        float availableHeight = Mathf.Max(1f, rect.height - (m.y + m.w));

        var current = new StringBuilder();

        for (int i = 0; i < words.Length; i++)
        {
            string w = words[i];

            if (current.Length == 0)
            {
                current.Append(w);
                continue;
            }

            string candidate = current.ToString() + " " + w;

            if (WouldOverflow(candidate, availableWidth, availableHeight))
            {
                pages.Add(current.ToString());
                current.Length = 0;
                current.Append(w);
            }
            else
            {
                current.Append(' ').Append(w);
            }
        }

        if (current.Length > 0)
            pages.Add(current.ToString());
    }


    private bool WouldOverflow(string candidate, float maxWidth, float maxHeight)
    {
        // Constrain by width; let TMP tell us required height.
        Vector2 needed = textComponent.GetPreferredValues(candidate, maxWidth, 0f);
        // Small slop to avoid off-by-a-few-pixels flicker
        return needed.y > (maxHeight + 0.5f);
    }
}
