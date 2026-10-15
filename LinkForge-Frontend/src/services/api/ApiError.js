export class ApiError extends Error {
  constructor(message, { status = 0, payload = null, cause } = {}) {
    super(message, { cause });
    this.name = 'ApiError';
    this.status = status;
    this.payload = payload;
  }
}
