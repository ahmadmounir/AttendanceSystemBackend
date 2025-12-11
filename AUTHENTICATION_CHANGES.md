# Authentication Changes - Summary

## Changes Made

### 1. Refresh Token Security ?
- **Before**: Refresh token was sent in JSON response body
- **After**: Refresh token stored in HTTP-only cookie
- **Benefit**: Prevents XSS attacks as JavaScript cannot access the token

### 2. Role Information ?
- **Before**: Response contained `roleId` (e.g., "role-1")
- **After**: Response contains `roleName` (e.g., "Administrator")
- **Benefit**: Frontend doesn't need extra API call to get role name

### 3. Consistent Error Messages ?
- **Before**: Various error messages for unauthorized access
- **After**: All unauthorized requests return:
  ```json
  {
    "success": false,
    "statusCode": 401,
    "message": "Not authorized",
    "data": null
  }
  ```

## Updated API Endpoints

### Login
**Endpoint**: `POST /api/v1/auth/login`

**Request**:
```json
{
  "username": "admin",
  "password": "password123"
}
```

**Response**:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "admin",
    "employeeId": "emp-123",
    "roleName": "Administrator",
    "expiresAt": "2025-12-10T11:30:00Z"
  }
}
```

**Cookie Set**:
```
Set-Cookie: refreshToken=<refresh-token>; HttpOnly; Secure; SameSite=Strict; Expires=<7-days-from-now>
```

### Refresh Token
**Endpoint**: `POST /api/v1/auth/refresh`

**Request**: No body needed (token read from cookie)

**Response**:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "admin",
    "employeeId": "emp-123",
    "roleName": "Administrator",
    "expiresAt": "2025-12-10T12:30:00Z"
  }
}
```

**Cookie Updated**:
```
Set-Cookie: refreshToken=<new-refresh-token>; HttpOnly; Secure; SameSite=Strict; Expires=<7-days-from-now>
```

### Logout
**Endpoint**: `POST /api/v1/auth/logout`

**Headers**: 
```
Authorization: Bearer <access-token>
```

**Response**:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Logout successful",
  "data": true
}
```

**Cookie Deleted**:
```
Set-Cookie: refreshToken=; Expires=<past-date>
```

## Frontend Integration

### Login Example
```javascript
const login = async (username, password) => {
  const response = await fetch('http://localhost:5079/api/v1/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include', // Important! Enables cookies
    body: JSON.stringify({ username, password }),
  });

  const result = await response.json();
  
  if (result.success) {
    // Store access token in memory or state
    localStorage.setItem('accessToken', result.data.accessToken);
    localStorage.setItem('username', result.data.username);
    localStorage.setItem('roleName', result.data.roleName);
    // Refresh token is automatically stored in HTTP-only cookie
  }
  
  return result;
};
```

### Refresh Token Example
```javascript
const refreshToken = async () => {
  const response = await fetch('http://localhost:5079/api/v1/auth/refresh', {
    method: 'POST',
    credentials: 'include', // Important! Sends cookie
  });

  const result = await response.json();
  
  if (result.success) {
    // Update access token
    localStorage.setItem('accessToken', result.data.accessToken);
  }
  
  return result;
};
```

### API Call with Auto-Refresh
```javascript
const apiCall = async (url, options = {}) => {
  // Add access token to request
  options.headers = {
    ...options.headers,
    'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
  };
  options.credentials = 'include';

  let response = await fetch(url, options);

  // If unauthorized, try to refresh token
  if (response.status === 401) {
    const refreshResult = await refreshToken();
    
    if (refreshResult.success) {
      // Retry original request with new token
      options.headers.Authorization = `Bearer ${localStorage.getItem('accessToken')}`;
      response = await fetch(url, options);
    } else {
      // Refresh failed, redirect to login
      window.location.href = '/login';
    }
  }

  return response.json();
};

// Usage
const employees = await apiCall('http://localhost:5079/api/v1/employees');
```

### Logout Example
```javascript
const logout = async () => {
  await fetch('http://localhost:5079/api/v1/auth/logout', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
    },
    credentials: 'include',
  });

  // Clear local storage
  localStorage.removeItem('accessToken');
  localStorage.removeItem('username');
  localStorage.removeItem('roleName');
  
  // Redirect to login
  window.location.href = '/login';
};
```

## Security Improvements

### HTTP-Only Cookies
- ? **XSS Protection**: JavaScript cannot access the refresh token
- ? **Secure Flag**: Cookie only sent over HTTPS (in production)
- ? **SameSite=Strict**: Protects against CSRF attacks
- ? **7-Day Expiration**: Automatic cleanup of old tokens

### Access Token
- ? Stored in memory/localStorage (your choice)
- ? Short-lived (1 hour)
- ? Included in Authorization header
- ? Automatically refreshed when expired

## Breaking Changes

### For API Consumers

1. **Login Response Changed**:
   - ? Removed: `refreshToken` field
   - ? Added: Refresh token in HTTP-only cookie
   - ? Removed: `roleId` field
   - ? Added: `roleName` field

2. **Refresh Token Endpoint Changed**:
   - ? Before: Required refresh token in request body
   - ? Now: Reads refresh token from cookie automatically

3. **Revoke Endpoint Renamed**:
   - ? Before: `/api/v1/auth/revoke`
   - ? Now: `/api/v1/auth/logout`

### Migration Guide

**Old Login**:
```javascript
// ? Old way
const result = await login(username, password);
const refreshToken = result.data.refreshToken; // Store somewhere
const roleId = result.data.roleId; // Need to fetch role name
```

**New Login**:
```javascript
// ? New way
const result = await login(username, password);
// refreshToken is in HTTP-only cookie automatically
const roleName = result.data.roleName; // Role name already included
```

**Old Refresh**:
```javascript
// ? Old way
const result = await fetch('/api/v1/auth/refresh', {
  method: 'POST',
  body: JSON.stringify({ refreshToken: storedRefreshToken }),
});
```

**New Refresh**:
```javascript
// ? New way
const result = await fetch('/api/v1/auth/refresh', {
  method: 'POST',
  credentials: 'include', // That's it!
});
```

## Testing with cURL

### Login
```bash
curl -X POST http://localhost:5079/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password123"}' \
  -c cookies.txt -v
```

### Refresh Token
```bash
curl -X POST http://localhost:5079/api/v1/auth/refresh \
  -b cookies.txt -c cookies.txt -v
```

### Protected Endpoint
```bash
curl -X GET http://localhost:5079/api/v1/employees \
  -H "Authorization: Bearer <access-token>" \
  -b cookies.txt
```

### Logout
```bash
curl -X POST http://localhost:5079/api/v1/auth/logout \
  -H "Authorization: Bearer <access-token>" \
  -b cookies.txt -c cookies.txt
```

## Database Changes

No database schema changes required! The system still stores refresh tokens the same way.

## Configuration

Ensure CORS is configured to allow credentials:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // ? Essential for cookies!
    });
});
```

## Summary

? **More Secure**: Refresh tokens protected from XSS  
? **Better UX**: Role name included in response  
? **Consistent**: All unauthorized errors use same format  
? **Simpler**: No need to manage refresh token in frontend  
? **Standards**: Follows OAuth 2.0 best practices
