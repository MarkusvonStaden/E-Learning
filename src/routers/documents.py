from fastapi import APIRouter, UploadFile, File, HTTPException
from langchain_community.document_loaders import PyPDFLoader
from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain_chroma import Chroma
from langchain_openai import OpenAIEmbeddings
import os
import dotenv
import logging

router = APIRouter()

logging.basicConfig(level=logging.INFO)

dotenv.load_dotenv()

@router.post("/documents/")
async def upload_document(file: UploadFile = File(...)):
    if file.content_type != "application/pdf":
        raise HTTPException(status_code=400, detail="File format not supported. Please upload a PDF file.")
    
    try:
        content = await file.read()
        file_path = f"/tmp/{file.filename}"
        
        with open(file_path, "wb") as f:
            f.write(content)
        
        logging.info(f"File written to {file_path}")
        
        pdf_loader = PyPDFLoader(file_path)
        pages = pdf_loader.load_and_split()
        
        logging.info(f"PDF loaded and split into pages")
        
        os.remove(file_path)
        
        text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=1000, chunk_overlap=200, add_start_index=True
        )
        
        chunks = text_splitter.split_documents(pages)
        
        logging.info(f"Document split into {len(chunks)} chunks")

        vectorstore = Chroma.from_documents(documents=chunks, embedding=OpenAIEmbeddings(), persist_directory="./chromadb")
        
        return len(chunks)
    except Exception as e:
        logging.error(f"Error processing file: {e}")
        raise HTTPException(status_code=500, detail="Error processing file")
    