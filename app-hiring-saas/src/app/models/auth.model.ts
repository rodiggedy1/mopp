import { LookupValue } from "./lookup.model";

export interface User {
  id: number;
  email: string;
  firstName?: string;
  lastName?: string;
  publicUsername?: string;
  phoneNumber?: string;
  profilePicture?: string;
  status: LookupValue;
  createdAt: Date;
  updatedAt: Date;
  type: string;
  dateCreated?: Date;
  picture?: string;
  suspensionReason?: string;
  categories: LookupValue[];
  subcategories: LookupValue[];
  receiveEmailNotifications?: boolean;
  receivePushNotifications?: boolean;
  calendlyProfileUrl?: string;
  calendlyDetails?: {
    clientId: string | null;
    clientSecret: string | null;
    accessToken: string | null;
    refreshToken: string | null;
    code: string | null;
    redirectUri: string | null;
    tokenExpiresAt?: string | null;
  }
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
  expiresIn: number;
}

export enum UserRole {
  VENDOR = 'Vendor',
  CLIENT = 'Customer',
  ADMIN = 'Administrator'
}

export interface TokenPayload {
  userId: string;
  email: string;
  role: UserRole;
  exp: number;
  iat: number;
}

export class Token {
  constructor(public accessToken: string, public refreshToken: string) {}
}

export class PasswordResetRequest {
  constructor(
    public token: string,
    public uid: string,
    public password: string
  ) {}
}

export class VerifyData {
  token?: string | null | undefined;
  uid?: string | null | undefined;
}

export class ResendEmail {
  constructor(
    public email ?: string
  ){}
}

export interface UserBase {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phoneNumber: string;
}

export interface UserRequest extends UserBase {
  publicUsername: string;
  role: string;
  companyDetails?: CompanyDetails;
}

export interface CompanyDetails {
  name: string;
  businessAddress: string;
  longitude: number;
  latitude: number;
  operatingRadius: number;
  contactPersonFirstName: string;
  contactPersonLastName: string;
  contactEmail: string;
  contactPhone: string;
  businessDescription: string;
  companySize: number;
  categoriesIds?: number[];
  subcategoriesIds?: number[];
}

export interface UserResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  picture: string;
  phoneNumber: string;
  suspensionReason: string;
  dateCreated: string;
  status: LookupValue;
  companyDetails: CompanyDetails;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

export interface ChangePasswordResponse {
  success: boolean;
  message?: string;
  data?: any;
}