export interface QuoteDto {
  quoteId: string;
  customerId: string;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  petId: string;
  petName: string;
  petType: string;
  breed: string;
  age: number;
  annualPremium: number;
  paymentFrequency: string;
  status: string;
  coverageType: string;
  annualLimit: number;
  deductible: number;
  reimbursementPct: number;
}
