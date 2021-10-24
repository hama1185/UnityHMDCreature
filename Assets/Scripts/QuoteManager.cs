using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuoteManager : MonoBehaviour
{
    // ここにセリフ書いて
    public string[] quoteListSection1 = {
        "この度は寄生生物の被験者実験に協力いただきありがとうございます",
        "本体験は、この場所 この時間でしか実施されないので、あなたはとてもラッキーですね",
        "今回の実験内容としましては、私たちが生み出した生物に寄生してもらい、その様子を観察するというものになります",
        "寄生のされ方には個人差があり、寄生生物とどれくらい適応できているかで現れる現象が変わります",
        "現れる現象は、身体に悪影響を与えるものではないので安心してください！",
        "それでは少し経過観察しましょうか..."
    };

    public string[] quoteListSection2 = {
        "ちょっと！入ってくるときはノックしてください！",
        "ごめんなさいね。驚かせちゃいましたよね",
        "そういえばなにか体に変化があったり、なにか見えるようになっていたりしますか？"
    };

    public string[] quoteListSection3 = {
        "これまでにほかの場所でも寄生実験をしているんですが、「助かりました」などの意見をいただけたりするんですよね～"
    };

    public string[] quoteListSection4 = {
        "うわっ！びっくりした～。大丈夫ですか？まったく...誰かが雑に置いたんですね...",
        "それでは、そろそろ寄生体験は終了のお時間となります",
        "最初から長時間の規制は身体の負担が大きいので今回の被験者実験は終了となります"
    };

    public string[] quoteListSection5 = {
        "これ以降は寄生されてはいないので気を付けてくださいね～"
    };

    public int quoteNumber = 0;
    
    // public GameObject quotebox;
    TMP_Text quote;
    TextMeshProSimpleAnimator textMeshProSimpleAnimator;
    
    void Start() {
        quote = this.GetComponent<TMP_Text>();
        textMeshProSimpleAnimator = this.GetComponent<TextMeshProSimpleAnimator>();
        quote.text = quoteListSection1[quoteNumber];
    }

    public void nextQuote(){
        quoteNumber += 1;
        quote.text = quoteListSection1[quoteNumber];
        textMeshProSimpleAnimator.Play();
    }

    public void beforeQuote(){
        quoteNumber -= 1;
        quote.text = quoteListSection1[quoteNumber];
        textMeshProSimpleAnimator.Play();
    }
}