import os
from flask import Flask, request, jsonify
from openai import OpenAI
from azure.search.documents import SearchClient
from azure.core.credentials import AzureKeyCredential

app = Flask(__name__)

client = OpenAI(api_key=os.getenv("OPENAI_API_KEY"))

search_client = SearchClient(
    endpoint=os.getenv("SEARCH_ENDPOINT"),
    index_name="docs-index",
    credential=AzureKeyCredential(os.getenv("SEARCH_API_KEY"))
)

@app.route("/")
def home():
    return """
    <h2>Company Policy Assistant</h2>
    <input id='q'/>
    <button onclick='ask()'>Ask</button>
    <pre id='a'></pre>

    <script>
    async function ask(){
        let q=document.getElementById('q').value
        let r=await fetch('/ask',{method:'POST',
        headers:{'Content-Type':'application/json'},
        body:JSON.stringify({question:q})})
        let d=await r.json()
        document.getElementById('a').innerText=d.answer
    }
    </script>
    """

@app.route("/ask", methods=["POST"])
def ask():

    question = request.json["question"]

    results = search_client.search(question)

    context = ""
    for r in results:
        context += r["content"]

    response = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[
            {"role":"system","content":"Answer using the company policy documents"},
            {"role":"user","content":question + context}
        ]
    )

    return jsonify({"answer":response.choices[0].message.content})


if __name__ == "__main__":
    app.run()
