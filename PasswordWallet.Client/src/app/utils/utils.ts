export const parseToDate = (dateString?: string): Date | null => {
  if (dateString) {
    const parsed = new Date(dateString);
    return isNaN(parsed.getTime()) ? null : parsed;
  }
  return null;
};

export const getPasswordInputType = (showPassword: boolean): string => {
  return showPassword ? 'text' : 'password';
};
