﻿using Auth.Core.Application.DTOs.Account;
using Auth.Core.Application.DTOs.Generic;
using Auth.Core.Application.Interfaces.Services;
using AutoMapper;

namespace Auth.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public UserService(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        public async Task<GenericApiResponse<AuthenticationResponse>> Login(AuthenticationRequest request)
        {
            GenericApiResponse<AuthenticationResponse> response = await _accountService.Authentication(request);
            return response;
        }
        public async Task SignOut()
        {
            await _accountService.SignOut();
        }
        public async Task<GenericApiResponse<RegisterResponse>> Register(RegisterRequest request, string origin)
        {
            var response = await _accountService.Register(request, origin);
            return response;
        }
        public async Task UpdateUser(string Id, RegisterRequest request)
        {
            await _accountService.UpdateUser(Id, request);
        }

        public async Task<string> EmailConfirm(string userId, string token)
        {
            return await _accountService.ConfirmEmail(userId, token);
        }
        public async Task<GenericApiResponse<string>> ForgotPassword(ForgotPasswordRequest request, string origin)
        {
            return await _accountService.ForgotPassword(request, origin);
        }
        public async Task<GenericApiResponse<string>> ResetPassword(ResetPasswordRequest request)
        {
            return await _accountService.ResetPassword(request);
        }
    }
}
