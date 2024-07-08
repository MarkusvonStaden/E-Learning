from fastapi import APIRouter, WebSocket, WebSocketDisconnect
from langchain.vectorstores.chroma import Chroma
from langchain.embeddings import OpenAIEmbeddings
from langchain.chat_models import ChatOpenAI
from langchain.prompts import ChatPromptTemplate

router = APIRouter()

PROMPT_TEMPLATE = """
Answer the question based only on the following context:

{context}

---

Chat History:
{chat_history}

Answer the question based on the above context: {question}
"""

class ConnectionManager:
    def __init__(self):
        self.active_connections: list[WebSocket] = []

    async def connect(self, websocket: WebSocket):
        await websocket.accept()
        self.active_connections.append(websocket)

    def disconnect(self, websocket: WebSocket):
        self.active_connections.remove(websocket)

embedding_function = OpenAIEmbeddings()
vector_store = Chroma(embedding_function=embedding_function, persist_directory='chromadb')

def ask(query, chat_history):
    results = vector_store.similarity_search_with_relevance_scores(query, 5)
    chat_model = ChatOpenAI(
        model = "gpt-4o"
    )
    chat_history_text = "\n".join([f"User: {entry['user']}\nAssistant: {entry['assistant']}" for entry in chat_history])
    if len(results) == 0 or results[0][1] < 0.7:
        prompt_template = ChatPromptTemplate.from_template("Chat History:\n{chat_history}\n\nUser: {question}\nAssistant:")
        prompt = prompt_template.format(chat_history=chat_history_text, question=query)
        response = chat_model.predict(prompt)
    else:
        context_text = "\n\n---\n\n".join([doc.page_content for doc, _score in results])
        prompt_template = ChatPromptTemplate.from_template(PROMPT_TEMPLATE)
        prompt = prompt_template.format(context=context_text, chat_history=chat_history_text, question=query)
        response = chat_model.predict(prompt)
    return response

manager = ConnectionManager()

@router.websocket("/ws/")
async def websocket_endpoint(websocket: WebSocket):
    await manager.connect(websocket)
    history = []
    try:
        while True:
            data = await websocket.receive_text()
            response = ask(data, history)
            await websocket.send_text(response)
            history.append({"user": data, "assistant": response})
    except WebSocketDisconnect:
        manager.disconnect(websocket)
