import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { QuoteDto } from '../models/quote.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class QuoteService {
  private baseUrl = `${environment.apiUrl}/api/quotes`;

  constructor(private http: HttpClient) {}

  getQuotes(status?: string): Observable<QuoteDto[]> {
    let params = new HttpParams();
    if (status) params = params.set('status', status);
    return this.http.get<QuoteDto[]>(this.baseUrl, { params });
  }

  getQuoteById(quoteId: string): Observable<QuoteDto> {
    return this.http.get<QuoteDto>(`${this.baseUrl}/${quoteId}`);
  }

  createQuote(dto: QuoteDto): Observable<QuoteDto> {
    return this.http.post<QuoteDto>(this.baseUrl, dto);
  }
}
