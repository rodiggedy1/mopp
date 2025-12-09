export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
}

export class paymentSendImprovement {
  constructor(
    public noteType ?: number,
    public message ?: string 
  ){}
}

export class paymentCancelAlert {
  constructor(
    public color ?: string,
    public message ?: string
  ){}
}

export class LookupValue {
  constructor(
    public id ?: number,
    public name ?: string
  ){}
}