import { client, useBackendQuery } from '@/lib/openpaymentmock-client';
import { useParams } from 'react-router-dom';
import PaymentNotFound from './components/payment-not-found';
import PaymentCompleted from './components/payment-completed';
import { useCallback, useEffect } from 'react';
import PaymentProcessing from './components/payment-processing';

export default function PaymentPage() {
  const { paymentId } = useParams();

  const {
    data: currentStatus,
    error,
    isLoading,
    mutate,
  } = useBackendQuery('/api/payments/{paymentId}/current-attempt', {
    params: {
      path: {
        paymentId: paymentId ?? '00000000-0000-0000-0000-000000000000',
      },
    },
  });

  const startPaymentAttempt = useCallback(async () => {
    await client.POST('/api/payments/{paymentId}/attempts/{attemptId}/start', {
      params: {
        path: {
          paymentId: paymentId ?? '00000000-0000-0000-0000-000000000000',
          attemptId:
            currentStatus?.id ?? '00000000-0000-0000-0000-000000000000',
        },
      },
    });

    mutate();
  }, [paymentId, currentStatus?.id, mutate]);

  const completePaymentSuccessfully = useCallback(async () => {
    await client.POST(
      '/api/payments/{paymentId}/attempts/{attemptId}/paid-successfully',
      {
        params: {
          path: {
            paymentId: paymentId ?? '00000000-0000-0000-0000-000000000000',
            attemptId:
              currentStatus?.id ?? '00000000-0000-0000-0000-000000000000',
          },
        },
      },
    );

    mutate();
  }, [paymentId, currentStatus?.id, mutate]);

  const completePaymentCancelled = useCallback(async () => {
    await client.POST(
      '/api/payments/{paymentId}/attempts/{attemptId}/payment-cancelled',
      {
        params: {
          path: {
            paymentId: paymentId ?? '00000000-0000-0000-0000-000000000000',
            attemptId:
              currentStatus?.id ?? '00000000-0000-0000-0000-000000000000',
          },
        },
      },
    );

    mutate();
  }, [paymentId, currentStatus?.id, mutate]);

  const completePaymentFailed = useCallback(
    async (error: string) => {
      await client.POST(
        '/api/payments/{paymentId}/attempts/{attemptId}/payment-issue',
        {
          params: {
            path: {
              paymentId: paymentId ?? '00000000-0000-0000-0000-000000000000',
              attemptId:
                currentStatus?.id ?? '00000000-0000-0000-0000-000000000000',
            },
            query: {
              error,
            },
          },
        },
      );

      mutate();
    },
    [paymentId, currentStatus?.id, mutate],
  );

  useEffect(() => {
    if (currentStatus?.status === 'NotAttempted') {
      startPaymentAttempt();
    }
  }, [currentStatus?.status, startPaymentAttempt]);

  if (error?.status === 404) {
    return <PaymentNotFound />;
  }

  if (
    isLoading ||
    currentStatus === undefined ||
    currentStatus.status === 'NotAttempted'
  ) {
    return <div>Loading...</div>;
  }

  if (
    currentStatus.paymentSituation.status === 'Succeeded' ||
    currentStatus.paymentSituation.status === 'Cancelled' ||
    currentStatus.paymentSituation.status === 'Failed'
  ) {
    return <PaymentCompleted payment={currentStatus.paymentSituation} />;
  }

  if (currentStatus.status === 'Started') {
    return (
      <PaymentProcessing
        {...{
          currentStatus,
          completePaymentSuccessfully,
          completePaymentCancelled,
          completePaymentFailed,
          reload: () => mutate(),
        }}
      />
    );
  }

  return (
    <div>
      <h1>Payment {paymentId}</h1>
      <p>Current status: {JSON.stringify(currentStatus, null, 2)}</p>
    </div>
  );
}
