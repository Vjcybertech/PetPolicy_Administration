export interface PolicyCoverageDto {
  policyCoverageId: number;
  policyId: number;
  coverageType: string;
  annualLimit: number;
  deductible: number;
  reimbursementPct: number;
}

export interface PolicyTransactionDto {
  transactionId: number;
  policyId: number;
  transactionType: string;
  effectiveDate: string;
  notes: string;
}

export interface PolicyRenewalDto {
  renewalId: number;
  policyId: number;
  previousEndDate: string;
  newStartDate: string;
  newEndDate: string;
  previousPremium: number;
  newPremium: number;
  renewalDate: string;
}

export interface PolicyDto {
  policyId: number;
  policyNumber: string;
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
  startDate: string;
  endDate: string;
  premium: number;
  paymentFrequency: string;
  status: string;
  coverages: PolicyCoverageDto[];
  transactions: PolicyTransactionDto[];
  renewals: PolicyRenewalDto[];
}

export interface CreatePolicyDto {
  quoteId: string;
  startDate: string;
  endDate: string;
  paymentFrequency: string;
  annualPremium: number;
  coverageType: string;
  annualLimit: number;
  deductible: number;
  reimbursementPct: number;
}

export interface RenewPolicyDto {
  proposedPremium: number;
  extensionYears: number;
  notes: string;
}

export interface CancelPolicyDto {
  cancellationDate: string;
  reason: string;
  notes: string;
}

export interface UpdatePolicyDto {
  customerEmail: string;
  customerPhone: string;
  paymentFrequency: string;
}
