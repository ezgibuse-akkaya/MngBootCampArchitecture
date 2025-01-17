﻿using Application.Services.Repositories;
using Core.CrossCuttingConcerns.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PromotionCodes.Rules
{
    public class PromotionCodeBusinessRules
    {
        private readonly IPromotionCodeRepository _promotionCodeRepository;

        public PromotionCodeBusinessRules(IPromotionCodeRepository promotionCodeRepository)
        {
            _promotionCodeRepository = promotionCodeRepository;
        }

        public async Task CheckIfPromotionCodeIsDuplicated(string code)
        {
            var result = await _promotionCodeRepository.GetListAsync(p => p.Code == code);

            if (result.Items.Any())
            {
                throw new BusinessException("Bu promosyon kodu daha önce oluşturulmuş.");
            }
        }

        public async Task CheckIfPromotionCodeExists(string code)
        {
            var result = await _promotionCodeRepository.GetAsync(p => p.Code == code);
            if (result is null)
            {
                throw new BusinessException("Böyle bir promosyon kodu bulunmamaktadır.");
            }
        } 

        public async Task CheckIfPromotionCodeIsUsed(string code, int customerId)
        {
            var result = await _promotionCodeRepository.GetAsync(c => c.Code == code && !c.Customers.Where(cu => cu.Id == customerId).Any() );

            if(result is null)
            {
                throw new BusinessException("Bu kullanıcı bu promosyon kodunu zaten kullanmış.");
            }
        }

        public async Task CheckIfPromotionCodeDateIsValid(string code)
        {
            var result = await _promotionCodeRepository.GetAsync(c => c.Code == code);

            if(result.ValidityDate.Date < DateTime.Now.Date)
            {
                throw new BusinessException("Promosyon kodunun tarihi geçmiş olduğu için geçersizdir.");
            }
        }
    }
}
