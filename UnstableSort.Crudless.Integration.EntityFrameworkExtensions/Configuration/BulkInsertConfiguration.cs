﻿using System;
using System.Linq.Expressions;
using UnstableSort.Crudless.Configuration;
using Z.BulkOperations;

namespace UnstableSort.Crudless.Integration.EntityFrameworkExtensions.Configuration
{
    public class BulkInsertConfiguration<TEntity> 
        : BulkConfiguration<TEntity, BulkInsertConfiguration<TEntity>>
        where TEntity : class
    {
        public override BulkOperation<TOperationEntity> Apply<TOperationEntity>(
            IRequestConfig config, 
            BulkOperation<TOperationEntity> operation)
        {
            operation = base.Apply(config, operation);
            
            if (IgnoredColumns.Count > 0)
            {
                if (operation.IgnoreOnInsertExpression != null)
                {
                    foreach (var member in ((NewExpression)operation.IgnoreOnInsertExpression.Body).Members)
                        IgnoredColumns.Add(member);
                }

                operation.IgnoreOnInsertExpression = CreateNewExpression<TOperationEntity>(IgnoredColumns);
            }

            return operation;
        }
        
        public BulkInsertConfiguration<TEntity> IgnoreColumns(
            params Expression<Func<TEntity, object>>[] members)
        {
            foreach (var memberExpr in members)
                foreach (var member in GetMembers(memberExpr))
                    IgnoredColumns.Add(member);

            return this;
        }
    }
}
