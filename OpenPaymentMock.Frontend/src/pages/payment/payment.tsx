import { client, useBackendQuery } from '@/lib/openpaymentmock-client';
import { useParams } from 'react-router-dom';
import PaymentNotFound from './components/payment-not-found';
import PaymentCompleted from './components/payment-completed';
import { useCallback, useEffect } from 'react';
import { Button } from '@/components/ui/button';

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

  console.log(currentStatus);
  console.log(error);

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
    await client.POST('/api/payments/{paymentId}/attempts/{attemptId}/paid-successfully', {
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

  
  const completePaymentCancelled = useCallback(async () => {
    await client.POST('/api/payments/{paymentId}/attempts/{attemptId}/payment-cancelled', {
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

  useEffect(() => {
    if (currentStatus?.status === 'NotAttempted') {
      startPaymentAttempt();
    }
  }, [currentStatus?.status, startPaymentAttempt]);

  if (error?.status === 404) {
    return <PaymentNotFound />;
  }

  if (error?.status === 400) {
    return <PaymentCompleted />;
  }

  if (
    isLoading ||
    currentStatus === undefined ||
    currentStatus.status === 'NotAttempted'
  ) {
    return <div>Loading...</div>;
  }

  if (currentStatus.status === 'Started') {
    return <div className='flex flex-col p-2'>
      <h1>Payment started!</h1>
      
      <div className='flex flex-row gap-2 mt-4'>
        <Button onClick={completePaymentSuccessfully}> 
          Successfull payment
        </Button>
        <Button onClick={completePaymentCancelled}>
          Failed payment
        </Button>
      </div>
    </div>;
  }

  return (
    <div>
      <h1>Payment {paymentId}</h1>
      <p>Current status: {JSON.stringify(currentStatus, null, 2)}</p>
    </div>
  );
}
