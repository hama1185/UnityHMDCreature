using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuoteManager : MonoBehaviour
{

    public AudioClip audioClip;
    AudioSource audioSource;
    public float scl = 3.0f;


    // ここにセリフ書いて
    string[] quoteListSection1 = {
        "この度は寄生生物の被験者実験に協力いただきありがとうございます",
        "本体験は、この場所 この時間でしか実施されないので、あなたはとてもラッキーです",
        "今回の実験内容としましては、私たちが生み出した生物に寄生してもらい、その様子を観察するというものになります",
        "寄生のされ方には個人差があり、寄生生物とどれくらい適応できているかで現れる現象が変わります",
        "現れる現象は、身体に悪影響を与えるものではないので安心してください！",
        "それでは少し経過観察しましょうか..."
    };

    string[] quoteListSection2 = {
        "ちょっと！入ってくるときはノックしてください！",
        "ごめんなさい。驚かせちゃいましたよね",
        "そういえばなにか体に変化があったり、なにか見えるようになっていたりしますか？"
    };

    string[] quoteListSection3 = {
        //"これまでにほかの場所でも寄生実験をしているんですが、「助かりました」などの意見をいただけたりするんですよね～"
        "何か変化があれば、それは寄生が始まっているということになります"
    };

    string[] quoteListSection4 = {
        "うわっ！びっくりした～。大丈夫ですか？まったく...誰かが雑に置いたんですね...",
        "それでは、そろそろ寄生体験は終了のお時間となります"
    };
    /*
    string[] quoteListSection5 = {
        "",
        "",
    };
    */

    string[] quoteListSection5 = {
        "最初から長時間の寄生は身体の負担が大きいので今回の被験者実験は終了となります",
        "これ以降は寄生されてはいないのでお気を付けください"
    };

    public int quoteNumber = 0;
    
    // public GameObject quotebox;
    public GameObject QuoteText;
    TMP_Text quote;
    TextMeshProSimpleAnimator textMeshProSimpleAnimator;
    
    void Start() {
        quote = QuoteText.GetComponent<TMP_Text>();
        textMeshProSimpleAnimator = QuoteText.GetComponent<TextMeshProSimpleAnimator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
    }

    public void nextQuote(int sectionNumber){
        switch(sectionNumber){
            case 0:
                quote.text = quoteListSection1[quoteNumber];
            break;
            case 1:
                quote.text = quoteListSection2[quoteNumber];
            break;
            case 2:
                quote.text = quoteListSection3[quoteNumber];
            break;
            case 3:
                quote.text = quoteListSection4[quoteNumber];
            break;
            case 4:
                quote.text = quoteListSection5[quoteNumber];
            break;
            //case 5:
                //quote.text = quoteListSection6[quoteNumber];
            //break;
            default:
            break;
        }
        quoteNumber += 1;
        textMeshProSimpleAnimator.Play();
        if(quote.text.Length > 1){
            audioSource.time = 6.0f * (1.2f - quote.text.Length / 53.0f);
            audioSource.Play();
            Debug.Log("tetetetete");
        }

    }

    public void resetQuoteNumber() {
        quoteNumber = 0;
    }

    // for文にぶち込む
    public int sectionMaxNumber(int sectionNumber) {
        int returnNumber = 0;
        switch(sectionNumber){
            case 0:
                returnNumber = quoteListSection1.Length;
            break;
            case 1:
                returnNumber = quoteListSection2.Length;
            break;
            case 2:
                returnNumber = quoteListSection3.Length;
            break;
            case 3:
                returnNumber = quoteListSection4.Length;
            break;
            case 4:
                returnNumber = quoteListSection5.Length;
            break;
            default:
            break;
        }
        return returnNumber;
    }
}