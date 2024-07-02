from fastapi import FastAPI
from .routers import documents
from .routers import completion

app = FastAPI()

app.include_router(documents.router)
app.include_router(completion.router)
