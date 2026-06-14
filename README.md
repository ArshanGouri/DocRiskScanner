# DocRiskScanner 📄

An AI-powered document risk analyzer built with ASP.NET Core MVC. Upload a contract, agreement, or policy document, and get an instant risk assessment — overall risk score, plain-English summary, and a breakdown of specific clauses that need attention.

## Features

- **PDF text extraction** — parses uploaded PDF documents using PdfPig
- **AI-powered risk analysis** — uses Groq's LLM API to identify risky, ambiguous, or non-standard clauses
- **Visual risk dashboard** — color-coded risk score (0-100) with a circular gauge
- **Severity-tagged issues** — each flagged clause is categorized as High, Medium, or Low risk with a plain-English explanation
- **Clean, responsive UI** — modern card-based design that works on desktop and mobile

## Tech Stack

- **Backend:** ASP.NET Core MVC (.NET 8), C#
- **PDF Processing:** PdfPig
- **AI:** Groq API (Llama 3.3 70B)
- **Frontend:** Razor views, custom CSS

## How It Works

1. User uploads a PDF document through the web interface
2. The backend extracts raw text from the PDF
3. The extracted text is sent to Groq's LLM with a structured prompt requesting risk analysis
4. The AI returns a JSON response containing an overall risk score, summary, and a list of flagged issues
5. Results are rendered as an interactive dashboard with color-coded severity indicators

## Getting Started

### Prerequisites

- .NET 8 SDK
- A free [Groq API key](https://console.groq.com)

### Setup

1. Clone the repository:
```bash
   git clone https://github.com/ArshanGouri/DocRiskScanner.git
   cd DocRiskScanner
```

2. Set your Groq API key using .NET User Secrets:
```bash
   dotnet user-secrets set "Groq:ApiKey" "your-groq-api-key-here"
```

3. Run the application:
```bash
   dotnet run
```

4. Open your browser and navigate to the local URL shown in the console.

## Use Cases

- Freelancers and small businesses reviewing service agreements before signing
- Quick first-pass review of contracts, NDAs, and rental agreements
- Identifying clauses that warrant a closer look or legal consultation

## Future Improvements

- Highlight risky clauses directly within the document text
- Support for DOCX and plain text uploads
- Export analysis reports as PDF

## License

This project is for portfolio/demonstration purposes.
